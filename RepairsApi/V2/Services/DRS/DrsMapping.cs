using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using NodaTime;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Extensions;
using RepairsApi.V2.Services.DRS;
using V2_Generated_DRS;
using RateScheduleItem = RepairsApi.V2.Infrastructure.RateScheduleItem;

namespace RepairsApi.V2.Services
{

    public class DrsMapping : IDrsMapping
    {
        private readonly IScheduleOfRatesGateway _sorGateway;
        private readonly IAlertsGateway _alertsGateway;
        private readonly ITenancyGateway _tenancyGateway;
        private readonly ISorPriorityGateway _sorPriorityGateway;

        public DrsMapping(
            IScheduleOfRatesGateway sorGateway,
            IAlertsGateway alertsGateway,
            ITenancyGateway tenancyGateway,
            ISorPriorityGateway sorPriorityGateway
        )
        {
            _sorGateway = sorGateway;
            _alertsGateway = alertsGateway;
            _tenancyGateway = tenancyGateway;
            _sorPriorityGateway = sorPriorityGateway;
        }

        public async Task<createOrder> BuildCreateOrderRequest(string sessionId, WorkOrder workOrder)
        {
            var createOrder = new createOrder
            {
                createOrder1 = new xmbCreateOrder
                {
                    sessionId = sessionId,
                    theOrder = await CreateOrder(workOrder)
                }
            };
            return createOrder;
        }

        private async Task<order> CreateOrder(WorkOrder workOrder)
        {
            var property = workOrder.Site?.PropertyClass.FirstOrDefault();
            var locationAlerts = property != null ? (await _alertsGateway.GetLocationAlertsAsync(property.PropertyReference)).Alerts : new List<Alert>();
            var tenureInfo = property != null ? await _tenancyGateway.GetTenancyInformationAsync(property.PropertyReference) : null;
            var personAlerts = tenureInfo != null ? (await _alertsGateway.GetPersonAlertsAsync(tenureInfo.TenancyAgreementReference)).Alerts : new List<Alert>();
            var uniqueCodes = locationAlerts?.Union(personAlerts);
            var orderComments =
                @$"{uniqueCodes.ToCodeString()} {workOrder.DescriptionOfWork}".Truncate(250);

            var orderCommentsExtended = $"Property Alerts {locationAlerts?.ToCommentsExtendedString()} " +
                                        $"Person Alerts {personAlerts?.ToCommentsExtendedString()}";

            var priorityCharacter = workOrder.WorkPriority.PriorityCode.HasValue
                ? await _sorPriorityGateway.GetLegacyPriorityCode(workOrder.WorkPriority.PriorityCode.Value)
                : ' ';


            return new order
            {
                status = orderStatus.PLANNED,
                primaryOrderNumber = workOrder.Id.ToString(CultureInfo.InvariantCulture),
                orderComments = orderComments,
                contract = workOrder.AssignedToPrimary.ContractorReference,
                locationID = workOrder.Site?.PropertyClass.FirstOrDefault()?.PropertyReference,
                priority = priorityCharacter.ToString(),
                targetDate =
                    workOrder.WorkPriority.RequiredCompletionDateTime.HasValue
                        ? DrsHelpers.ConvertToDrsTimeZone(workOrder.WorkPriority.RequiredCompletionDateTime.Value)
                        : DateTime.UtcNow,
                userId = workOrder.AgentEmail ?? workOrder.AgentName,
                contactName = workOrder.Customer.Name,
                phone = workOrder.Customer.Person.Communication.GetPhoneNumber(),
                orderCommentsExtended = orderCommentsExtended,
                theLocation = new location
                {
                    locationId = workOrder.Site?.PropertyClass.FirstOrDefault()?.PropertyReference,
                    name = workOrder.Site?.Name,
                    address1 = workOrder.Site?.PropertyClass.FirstOrDefault()?.Address.AddressLine,
                    postCode = workOrder.Site?.PropertyClass.FirstOrDefault()?.Address.PostalCode,
                    contract = workOrder.AssignedToPrimary.ContractorReference,
                    citizensName = workOrder.Customer.Name
                },
                theBookingCodes = await BuildBookingCodes(workOrder),
            };
        }

        public async Task<deleteOrder> BuildDeleteOrderRequest(string sessionId, WorkOrder workOrder)
        {
            var deleteOrder = new deleteOrder
            {
                deleteOrder1 = new xmbDeleteOrder
                {
                    sessionId = sessionId,
                    theOrder = await CreateOrder(workOrder)
                }
            };
            return deleteOrder;
        }

        public Task<updateBooking> BuildCompleteOrderUpdateBookingRequest(string sessionId, WorkOrder workOrder, order drsOrder)
        {
            var booking = drsOrder.theBookings.First();
            booking.theOrder = drsOrder;
            booking.theOrder.theBusinessData = SetBusinessData(booking.theOrder.theBusinessData, DrsBusinessDataNames.Status, DrsBookingStatusCodes.Completed);
            booking.theOrder.theBookings = null;
            booking.theOrder.status = orderStatus.COMPLETED;
            booking.bookingCompletionStatus = DrsBookingStatusCodes.Completed;
            booking.bookingLifeCycleStatus = transactionTypeType.COMPLETED;
            booking.theBusinessData = SetBusinessData(booking.theBusinessData, DrsBusinessDataNames.TaskLifeCycleStatus, DrsTaskLifeCycleCodes.Completed);
            booking.theBookingCodes?.ToList().ForEach(bc => bc.itemValue = null);
            var resource = booking.theResources?.First();

            var updateBooking = new updateBooking
            {
                updateBooking1 = new xmbUpdateBooking
                {
                    completeOrder = true,
                    startDateAndTime = booking.assignedStart,
                    endDateAndTime = booking.assignedEnd,
                    resourceId = resource?.resourceID,
                    transactionType = transactionTypeType.COMPLETED,
                    sessionId = sessionId,
                    theBooking = booking,
                }
            };


            return Task.FromResult(updateBooking);
        }

        private static businessData[] SetBusinessData(businessData[] businessData, string name, string value)
        {
            var existingValue = businessData?.SingleOrDefault(bd => bd.name == name);

            if (existingValue is null)
            {
                businessData ??= Array.Empty<businessData>();
                businessData = businessData.Concat(new[]
                {
                    new businessData
                    {
                        name = name, value = value
                    }
                }).ToArray();
            }
            else
            {
                existingValue.value = value;
            }

            return businessData;
        }

        private async Task<bookingCode[]> BuildBookingCodes(WorkOrder workOrder)
        {
            var bookingIndex = 1;
            var rateScheduleItems = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem).ToList();

            var bookings = new List<bookingCode>();
            foreach (var rateScheduleItem in rateScheduleItems)
            {
                var bookingCode = await CreateBookingCode(workOrder, rateScheduleItem, bookingIndex++);
                bookings.Add(bookingCode);
            }

            return bookings.ToArray();
        }

        private async Task<bookingCode> CreateBookingCode(WorkOrder workOrder, RateScheduleItem rsi, int index)
        {
            var sorCode = await _sorGateway.GetCode(
                rsi.CustomCode,
                workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference,
                workOrder.AssignedToPrimary.ContractorReference
            );

            return new bookingCode
            {
                primaryOrderNumber = workOrder.Id.ToString(CultureInfo.InvariantCulture),
                quantity = rsi.Quantity.Amount.ToString(CultureInfo.InvariantCulture),
                bookingCodeSORCode = sorCode.Code,
                bookingCodeDescription = sorCode.LongDescription ?? sorCode.ShortDescription,
                itemNumberWithinBooking = index.ToString(CultureInfo.InvariantCulture),
                trade = sorCode.TradeCode,
                standardMinuteValue = sorCode.StandardMinuteValue.ToString()
            };
        }
    }
}
