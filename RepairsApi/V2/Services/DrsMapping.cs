using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Extensions;
using V2_Generated_DRS;
using RateScheduleItem = RepairsApi.V2.Infrastructure.RateScheduleItem;

namespace RepairsApi.V2.Services
{

    public class DrsMapping : IDrsMapping
    {
        private readonly IScheduleOfRatesGateway _sorGateway;
        private readonly IAlertsGateway _alertsGateway;
        private readonly ISorPriorityGateway _sorPriorityGateway;

        public DrsMapping(
            IScheduleOfRatesGateway sorGateway,
            IAlertsGateway alertsGateway,
            ISorPriorityGateway sorPriorityGateway
        )
        {
            _sorGateway = sorGateway;
            _alertsGateway = alertsGateway;
            _sorPriorityGateway = sorPriorityGateway;
        }

        public async Task<createOrder> BuildCreateOrderRequest(string sessionId, WorkOrder workOrder)
        {
            var property = workOrder.Site.PropertyClass.FirstOrDefault();
            var locationAlerts = property != null ? await _alertsGateway.GetLocationAlertsAsync(property?.PropertyReference) : null;

            char priorityCharacter = workOrder.WorkPriority.PriorityCode.HasValue
                ? await _sorPriorityGateway.GetLegacyPriorityCode(workOrder.WorkPriority.PriorityCode.Value)
                : ' ';

            var createOrder = new createOrder
            {
                createOrder1 = new xmbCreateOrder
                {
                    sessionId = sessionId,
                    theOrder = new order
                    {
                        status = orderStatus.PLANNED,
                        primaryOrderNumber = workOrder.Id.ToString(CultureInfo.InvariantCulture),
                        orderComments = workOrder.DescriptionOfWork,
                        contract = workOrder.AssignedToPrimary.ContractorReference,
                        locationID = workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference,
                        priority = priorityCharacter.ToString(),
                        targetDate = workOrder.WorkPriority.RequiredCompletionDateTime ?? DateTime.UtcNow,
                        userId = workOrder.AgentEmail ?? workOrder.AgentName,
                        contactName = workOrder.Customer.Name,
                        phone = workOrder.Customer.Person.Communication.GetPhoneNumber(),
                        theLocation = new location
                        {
                            locationId = workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference,
                            name = workOrder.Site.Name,
                            address1 = workOrder.Site.PropertyClass.FirstOrDefault()?.Address.AddressLine,
                            postCode = workOrder.Site.PropertyClass.FirstOrDefault()?.Address.PostalCode,
                            contract = workOrder.AssignedToPrimary.ContractorReference,
                            citizensName = workOrder.Customer.Name,
                            theLocationLines = locationAlerts?.Alerts.Select(a => new locationLine
                            {
                                citizensName = workOrder.Customer.Name,
                                lineCode = a.AlertCode,
                                lineDescription = a.Description
                            }).ToArray<locationLine>()
                        },
                        theBookingCodes = await BuildBookingCodes(workOrder),
                    }
                }
            };
            return createOrder;
        }

        private async Task<bookingCode[]> BuildBookingCodes(WorkOrder workOrder)
        {
            var bookingIndex = 1;
            var workElement = workOrder.WorkElements.FirstOrDefault();
            var bookingTasks = workElement?
                .RateScheduleItem.Select(async rsi => await CreateBookingCode(workOrder, rsi, bookingIndex++));
            var bookings = await Task.WhenAll(bookingTasks);
            return bookings;
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
                itemValue = sorCode.Cost?.ToString(CultureInfo.InvariantCulture) ?? "0",
                itemNumberWithinBooking = index.ToString(CultureInfo.InvariantCulture),
                trade = sorCode.TradeCode,
                standardMinuteValue = sorCode.StandardMinuteValue.ToString()
            };
        }
    }
}
