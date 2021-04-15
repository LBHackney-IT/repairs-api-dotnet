using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Extensions;
using RepairsApi.V2.Services;
using V2_Generated_DRS;
using RateScheduleItem = RepairsApi.V2.Infrastructure.RateScheduleItem;

namespace RepairsApi.Tests.V2.Services
{
    public class DrsMappingTests
    {
        private Mock<IScheduleOfRatesGateway> _sorGatewayMock;
        private DrsMapping _classUnderTest;
        private string _sessionId;
        private Mock<IAlertsGateway> _alertsGatewayMock;
        private PropertyAlertList _locationAlerts;

        [SetUp]
        public void Setup()
        {
            _sessionId = "sessionId";
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _alertsGatewayMock = new Mock<IAlertsGateway>();
            _classUnderTest = new DrsMapping(_sorGatewayMock.Object, _alertsGatewayMock.Object);

            var generator = new Generator<PropertyAlertList>().AddDefaultGenerators();
            _locationAlerts = generator.Generate();

            _alertsGatewayMock.Setup(x => x.GetLocationAlertsAsync(It.IsAny<string>()))
                .ReturnsAsync(_locationAlerts);
        }

        [TestCase(WorkPriorityCode._1, "I")]
        [TestCase(WorkPriorityCode._2, "I")]
        [TestCase(WorkPriorityCode._3, "E")]
        [TestCase(WorkPriorityCode._4, "U")]
        [TestCase(WorkPriorityCode._5, "N")]
        public async Task MapsPriorityCorrectly(WorkPriorityCode incomingCode, string expectedDrsCode)
        {
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();
            workOrder.WorkPriority.PriorityCode = incomingCode;
            var sorCodes = SetupSorCodes(workOrder);

            var request = await _classUnderTest.BuildCreateOrderRequest(_sessionId, workOrder);

            VerifyCreateOrder(request, workOrder, sorCodes);
            request.createOrder1.theOrder.priority.Should().Be(expectedDrsCode);
        }

        private IList<ScheduleOfRatesModel> SetupSorCodes(WorkOrder workOrder)
        {
            var sorCodes = workOrder.WorkElements.FirstOrDefault()?.RateScheduleItem
                .Select(rsi => new ScheduleOfRatesModel
                {
                    Code = rsi.CustomCode,
                    ShortDescription = $"{rsi.CustomCode} short description",
                    LongDescription = $"{rsi.CustomCode} long description"
                }).ToList();

            if (sorCodes == null) return null;

            foreach (var code in sorCodes)
            {
                _sorGatewayMock.Setup(x => x.GetCode(code.Code, It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(code);
            }

            return sorCodes;
        }

        private void VerifyCreateOrder(createOrder createOrder, WorkOrder workOrder, IList<ScheduleOfRatesModel> sorCodes)
        {
            createOrder.createOrder1.sessionId.Should().Be(_sessionId);
            ValidateOrder(workOrder, createOrder.createOrder1.theOrder, sorCodes, _locationAlerts);
        }

        private static void ValidateOrder(WorkOrder workOrder, order order, IList<ScheduleOfRatesModel> sorCodes, PropertyAlertList locationAlerts)
        {
            order.primaryOrderNumber.Should().Be(workOrder.Id.ToString(CultureInfo.InvariantCulture));
            order.status.Should().Be(orderStatus.PLANNED);
            order.primaryOrderNumber.Should().Be(workOrder.Id.ToString(CultureInfo.InvariantCulture));
            order.orderComments.Should().Be(workOrder.DescriptionOfWork);
            order.contract.Should().Be(workOrder.AssignedToPrimary.ContractorReference);
            order.locationID.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference);
            order.targetDate.Should().Be(workOrder.WorkPriority.RequiredCompletionDateTime!.Value);
            order.userId.Should().Be(workOrder.AgentEmail);
            order.contactName.Should().Be(workOrder.Customer.Name);
            order.phone.Should().Be(workOrder.Customer.Person.Communication.GetPhoneNumber());

            ValidateLocation(workOrder, locationAlerts, order.theLocation);

            ValidateBookings(workOrder, sorCodes, order.theBookingCodes);
        }

        private static void ValidateBookings(WorkOrder workOrder, IList<ScheduleOfRatesModel> sorCodes, bookingCode[] bookings)
        {
            var workElement = workOrder.WorkElements.FirstOrDefault();

            foreach (var (booking, index) in bookings.WithIndex())
            {
                var sorCode = sorCodes.Single(c => c.Code == booking.bookingCodeSORCode);
                var rateScheduleItem = workElement!.RateScheduleItem.Single(rsi => rsi.CustomCode == booking.bookingCodeSORCode);
                ValidateBooking(
                    workOrder,
                    rateScheduleItem,
                    sorCode,
                    booking,
                    index
                );
            }
        }

        private static void ValidateBooking(WorkOrder workOrder, RateScheduleItem rateScheduleItem, ScheduleOfRatesModel sorCode, bookingCode booking, int index)
        {
            booking.primaryOrderNumber.Should().Be(workOrder.Id.ToString(CultureInfo.InvariantCulture));
            booking.quantity.Should().Be(rateScheduleItem.Quantity.Amount.ToString(CultureInfo.InvariantCulture));
            booking.bookingCodeSORCode.Should().Be(sorCode.Code);
            booking.bookingCodeDescription.Should().Be(sorCode.LongDescription ?? sorCode.ShortDescription);
            booking.itemValue.Should().Be(sorCode.Cost?.ToString(CultureInfo.InvariantCulture) ?? "0");
            booking.itemNumberWithinBooking.Should().Be((index + 1).ToString(CultureInfo.InvariantCulture));
            booking.trade.Should().Be(sorCode.TradeCode);
        }

        private static void ValidateLocation(WorkOrder workOrder, PropertyAlertList locationAlerts, location location)
        {
            location.locationId.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference);
            location.name.Should().Be(workOrder.Site.Name);
            location.address1.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.Address.AddressLine);
            location.postCode.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.Address.PostalCode);
            location.contract.Should().Be(workOrder.AssignedToPrimary.ContractorReference);
            location.citizensName.Should().Be(workOrder.Customer.Name);
            location.theLocationLines.Should().BeEquivalentTo<locationLine>(locationAlerts.Alerts.Select(a => new locationLine
            {
                citizensName = workOrder.Customer.Name,
                lineCode = a.AlertCode,
                lineDescription = a.Description
            }).ToArray());
        }
    }
}
