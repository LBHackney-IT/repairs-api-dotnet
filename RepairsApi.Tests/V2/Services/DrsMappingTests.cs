using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NodaTime;
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
        private Mock<ISorPriorityGateway> _sorPriorityGatewayMock;
        private PropertyAlertList _locationAlerts;
        private PersonAlertList _personAlerts;
        private Mock<ITenancyGateway> _tenancyGateway;
        private TenureInformation _tenureInformation;

        [SetUp]
        public void Setup()
        {
            _sessionId = "sessionId";
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _alertsGatewayMock = new Mock<IAlertsGateway>();
            _tenancyGateway = new Mock<ITenancyGateway>();
            _sorPriorityGatewayMock = new Mock<ISorPriorityGateway>();
            _classUnderTest = new DrsMapping(_sorGatewayMock.Object, _alertsGatewayMock.Object, _tenancyGateway.Object, _sorPriorityGatewayMock.Object);

            _locationAlerts = new Generator<PropertyAlertList>().AddDefaultGenerators().Generate();
            _personAlerts = new Generator<PersonAlertList>().AddDefaultGenerators().Generate();
            _tenureInformation = new Generator<TenureInformation>().AddDefaultGenerators().Generate();

            _alertsGatewayMock.Setup(x => x.GetLocationAlertsAsync(It.IsAny<string>()))
                .ReturnsAsync(_locationAlerts);
            _alertsGatewayMock.Setup(x => x.GetPersonAlertsAsync(It.IsAny<string>()))
                .ReturnsAsync(_personAlerts);
            _tenancyGateway.Setup(x => x.GetTenancyInformationAsync(It.IsAny<string>()))
                .ReturnsAsync(_tenureInformation);
        }

        [TestCase('I')]
        [TestCase('E')]
        [TestCase('U')]
        [TestCase('N')]
        public async Task MapsPriorityCorrectly(char expectedDrsCode)
        {
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();
            _sorPriorityGatewayMock.Setup(m => m.GetLegacyPriorityCode(It.IsAny<int>())).ReturnsAsync(expectedDrsCode);
            var sorCodes = SetupSorCodes(workOrder);

            var request = await _classUnderTest.BuildCreateOrderRequest(_sessionId, workOrder);

            VerifyCreateOrder(request, workOrder, sorCodes);
            request.createOrder1.theOrder.priority.Should().Be(expectedDrsCode.ToString());
        }

        [Test]
        public async Task CreatesDelete()
        {
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();
            var sorCodes = SetupSorCodes(workOrder);

            var request = await _classUnderTest.BuildDeleteOrderRequest(_sessionId, workOrder);

            VerifyDeleteOrder(request, workOrder, sorCodes);
        }

        [Test]
        public async Task MapsAlerts()
        {
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();
            var workOrder = generator.Generate();
            _sorPriorityGatewayMock.Setup(m => m.GetLegacyPriorityCode(It.IsAny<int>())).ReturnsAsync('I');
            var sorCodes = SetupSorCodes(workOrder);

            var request = await _classUnderTest.BuildCreateOrderRequest(_sessionId, workOrder);

            _alertsGatewayMock.Verify(x => x.GetLocationAlertsAsync(workOrder.Site.PropertyClass.First().PropertyReference));
            _alertsGatewayMock.Verify(x => x.GetPersonAlertsAsync(_tenureInformation.TenancyAgreementReference));
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
            ValidateOrder(workOrder, createOrder.createOrder1.theOrder, sorCodes, _locationAlerts, _personAlerts);
        }

        private void VerifyDeleteOrder(deleteOrder deleteOrder, WorkOrder workOrder, IList<ScheduleOfRatesModel> sorCodes)
        {
            deleteOrder.deleteOrder1.sessionId.Should().Be(_sessionId);
            ValidateOrder(workOrder, deleteOrder.deleteOrder1.theOrder, sorCodes, _locationAlerts, _personAlerts);
        }

        private static void ValidateOrder(WorkOrder workOrder, order order, IList<ScheduleOfRatesModel> sorCodes, PropertyAlertList locationAlerts, PersonAlertList personAlertList)
        {
            order.primaryOrderNumber.Should().Be(workOrder.Id.ToString(CultureInfo.InvariantCulture));
            order.status.Should().Be(orderStatus.PLANNED);
            order.primaryOrderNumber.Should().Be(workOrder.Id.ToString(CultureInfo.InvariantCulture));
            order.orderComments.Should().Be(workOrder.DescriptionOfWork);
            order.contract.Should().Be(workOrder.AssignedToPrimary.ContractorReference);
            order.locationID.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference);
            order.userId.Should().Be(workOrder.AgentEmail);
            order.contactName.Should().Be(workOrder.Customer.Name);
            order.phone.Should().Be(workOrder.Customer.Person.Communication.GetPhoneNumber());

            var expectedExtendedComments = $"--- Property Alerts ---{locationAlerts.Alerts.ToDescriptionString()}{Environment.NewLine}" +
                                           $"--- Person Alerts ---{personAlertList.Alerts.ToDescriptionString()}";
            order.orderCommentsExtended.Should().Be(expectedExtendedComments);

            ValidateLocation(workOrder, order.theLocation);
            ValidateBookings(workOrder, sorCodes, order.theBookingCodes);
            ValidateTargetDate(workOrder.WorkPriority.RequiredCompletionDateTime!.Value, order.targetDate);
        }

        private static void ValidateTargetDate(DateTime requiredCompletionDateTime, DateTime targetDate)
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var local = Instant.FromDateTimeUtc(requiredCompletionDateTime).InUtc();
            targetDate.Should().Be(local.WithZone(london).ToDateTimeUnspecified());
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
            booking.standardMinuteValue.Should().Be(sorCode.StandardMinuteValue.ToString());
        }

        private static void ValidateLocation(WorkOrder workOrder, location location)
        {
            location.locationId.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference);
            location.name.Should().Be(workOrder.Site.Name);
            location.address1.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.Address.AddressLine);
            location.postCode.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.Address.PostalCode);
            location.contract.Should().Be(workOrder.AssignedToPrimary.ContractorReference);
            location.citizensName.Should().Be(workOrder.Customer.Name);
        }
    }
}
