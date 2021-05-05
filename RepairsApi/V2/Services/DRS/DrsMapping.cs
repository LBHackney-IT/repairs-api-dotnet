using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using RepairsApi.V2.Domain;
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
            var locationAlerts = property != null ? await _alertsGateway.GetLocationAlertsAsync(property.PropertyReference) : null;
            var tenureInfo = property != null ? await _tenancyGateway.GetTenancyInformationAsync(property.PropertyReference) : null;
            var personAlerts = tenureInfo != null ? await _alertsGateway.GetPersonAlertsAsync(tenureInfo.TenancyAgreementReference) : null;
            var orderCommentsExtended = $"Property Alerts {locationAlerts?.Alerts.ToDescriptionString()} " +
                                        $"Person Alerts {personAlerts?.Alerts.ToDescriptionString()}";

            char priorityCharacter = workOrder.WorkPriority.PriorityCode.HasValue
                ? await _sorPriorityGateway.GetLegacyPriorityCode(workOrder.WorkPriority.PriorityCode.Value)
                : ' ';


            return new order
            {
                status = orderStatus.PLANNED,
                primaryOrderNumber = workOrder.Id.ToString(CultureInfo.InvariantCulture),
                orderComments = workOrder.DescriptionOfWork,
                contract = workOrder.AssignedToPrimary.ContractorReference,
                locationID = workOrder.Site?.PropertyClass.FirstOrDefault()?.PropertyReference,
                priority = priorityCharacter.ToString(),
                targetDate =
                    workOrder.WorkPriority.RequiredCompletionDateTime.HasValue
                        ? ConvertToDrsTimeZone(workOrder.WorkPriority.RequiredCompletionDateTime.Value)
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

        private static DateTime ConvertToDrsTimeZone(DateTime dateTime)
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var utcDateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
            var local = Instant.FromDateTimeUtc(utcDateTime).InUtc();
            return local.WithZone(london).ToDateTimeUnspecified();
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