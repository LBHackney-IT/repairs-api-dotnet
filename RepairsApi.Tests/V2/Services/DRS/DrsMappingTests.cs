using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using NodaTime;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Extensions;
using RepairsApi.V2.Services;
using RepairsApi.V2.Services.DRS;
using V2_Generated_DRS;
using RateScheduleItem = RepairsApi.V2.Infrastructure.RateScheduleItem;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

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
        private Helpers.StubGeneration.Generator<WorkOrder> _workOrderGenerator;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _workOrderGenerator = new Helpers.StubGeneration.Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators();

            _sessionId = "sessionId";
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _alertsGatewayMock = new Mock<IAlertsGateway>();
            _tenancyGateway = new Mock<ITenancyGateway>();
            _sorPriorityGatewayMock = new Mock<ISorPriorityGateway>();
            _classUnderTest = new DrsMapping(_sorGatewayMock.Object, _alertsGatewayMock.Object, _tenancyGateway.Object, _sorPriorityGatewayMock.Object);

            _locationAlerts = new Helpers.StubGeneration.Generator<PropertyAlertList>().AddDefaultGenerators().Generate();
            _personAlerts = new Helpers.StubGeneration.Generator<PersonAlertList>().AddDefaultGenerators().Generate();
            var commonAlerts = new Helpers.StubGeneration.Generator<Alert>().AddDefaultGenerators().GenerateList(5);
            _locationAlerts.Alerts = _locationAlerts.Alerts.Union(commonAlerts);
            _personAlerts.Alerts = _personAlerts.Alerts.Union(commonAlerts);

            _tenureInformation = new Helpers.StubGeneration.Generator<TenureInformation>().AddDefaultGenerators().Generate();

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
            var workOrder = _workOrderGenerator.Generate();
            _sorPriorityGatewayMock.Setup(m => m.GetLegacyPriorityCode(It.IsAny<int>())).ReturnsAsync(expectedDrsCode);
            var sorCodes = SetupSorCodes(workOrder);

            var request = await _classUnderTest.BuildCreateOrderRequest(_sessionId, workOrder);

            VerifyCreateOrder(request, workOrder, sorCodes);
            request.createOrder1.theOrder.priority.Should().Be(expectedDrsCode.ToString());
        }

        [Test]
        public async Task CreatesDelete()
        {
            var workOrder = _workOrderGenerator.Generate();
            var sorCodes = SetupSorCodes(workOrder);

            var request = await _classUnderTest.BuildDeleteOrderRequest(_sessionId, workOrder);

            VerifyDeleteOrder(request, workOrder, sorCodes);
        }

        [Test]
        public async Task CreatesCompleteOrder()
        {
            var expectedOrderNumber = _fixture.Create<int>();
            var drsOrder = GenerateDrsOrder(expectedOrderNumber);
            var workOrder = GenerateWorkOrder(drsOrder);
            var sorCodes = SetupSORCodes(drsOrder);

            var drsOrderCopy = drsOrder.DeepClone();
            var request = await _classUnderTest.BuildCompleteOrderUpdateBookingRequest(_sessionId, workOrder, drsOrderCopy);

            VerifyCompleteOrder(request, workOrder, sorCodes, drsOrder);
        }

        [Test]
        public async Task CreatesCompleteOrderAddsBusinessDataWhenItemNotPresent()
        {
            var expectedOrderNumber = _fixture.Create<int>();
            var drsOrder = GenerateDrsOrder(expectedOrderNumber);
            drsOrder.theBusinessData = new[]{new businessData
            {
                name = "WRONG_NAME", value = "WRONG_VALUE"
            }};
            var workOrder = GenerateWorkOrder(drsOrder);
            SetupSORCodes(drsOrder);

            var request = await _classUnderTest.BuildCompleteOrderUpdateBookingRequest(_sessionId, workOrder, drsOrder);

            ValidateBusinessData(request.updateBooking1.theBooking.theOrder.theBusinessData, DrsBusinessDataNames.Status, DrsBookingStatusCodes.Completed);
        }

        [Test]
        public async Task CreatesCompleteOrderAddsBusinessDataWhenListNotPresent()
        {
            var expectedOrderNumber = _fixture.Create<int>();
            var drsOrder = GenerateDrsOrder(expectedOrderNumber);
            drsOrder.theBusinessData = null;
            var workOrder = GenerateWorkOrder(drsOrder);
            SetupSORCodes(drsOrder);

            var request = await _classUnderTest.BuildCompleteOrderUpdateBookingRequest(_sessionId, workOrder, drsOrder);

            ValidateBusinessData(request.updateBooking1.theBooking.theOrder.theBusinessData, DrsBusinessDataNames.Status, DrsBookingStatusCodes.Completed);
        }

        [Test]
        public async Task CreatesCompleteOrderUpdatesBusinessDataWhenAlreadyPresent()
        {
            var expectedOrderNumber = _fixture.Create<int>();
            var drsOrder = GenerateDrsOrder(expectedOrderNumber);
            drsOrder.theBusinessData = new[]{new businessData
            {
                name = DrsBusinessDataNames.Status, value = "WRONG_VALUE"
            }};
            var workOrder = GenerateWorkOrder(drsOrder);
            SetupSORCodes(drsOrder);

            var request = await _classUnderTest.BuildCompleteOrderUpdateBookingRequest(_sessionId, workOrder, drsOrder);

            ValidateBusinessData(request.updateBooking1.theBooking.theOrder.theBusinessData, DrsBusinessDataNames.Status, DrsBookingStatusCodes.Completed);
        }

        private static WorkOrder GenerateWorkOrder(order drsOrder)
        {
            var booking = drsOrder.theBookings.First();
            var bookingCodes = booking.theBookingCodes;

            var workElements = new List<WorkElement>
            {
                new WorkElement
                {
                    RateScheduleItem = bookingCodes.Select(b => new RateScheduleItem
                    {
                        CustomCode = b.bookingCodeSORCode,
                        Quantity = new Quantity
                        {
                            Amount = int.Parse(b.quantity)
                        }
                    }).ToList()
                }
            };

            var workOrder = new WorkOrder
            {
                WorkElements = workElements,
                Id = int.Parse(drsOrder.primaryOrderNumber),
                DescriptionOfWork = drsOrder.orderComments,
                AgentEmail = drsOrder.userId,
                AssignedToPrimary = new Party
                {
                    ContractorReference = drsOrder.contract
                },
                Customer = new Party
                {
                    Name = drsOrder.contactName,
                    Person = new Person
                    {
                        Communication = new List<Communication>()
                    }
                },
                Site = new Site
                {
                    Name = drsOrder.theLocation.name,
                    PropertyClass = new List<PropertyClass>
                    {
                        new PropertyClass
                        {
                            PropertyReference = drsOrder.locationID,
                            Address = new PropertyAddress
                            {
                                AddressLine = drsOrder.theLocation.address1,
                                PostalCode = drsOrder.theLocation.postCode
                            }
                        }
                    }
                }
            };
            return workOrder;
        }

        private order GenerateDrsOrder(int expectedOrderNumber)
        {
            var index = 1;
            var bookingCodesGenerator = new Helpers.StubGeneration.Generator<bookingCode[]>()
                .AddDefaultGenerators()
                .AddValue(_fixture.Create<int>().ToString(), (bookingCode bc) => bc.quantity)
                .AddValue(_fixture.Create<int>().ToString(), (bookingCode bc) => bc.standardMinuteValue)
                .AddValue(_fixture.Create<int>().ToString(), (bookingCode bc) => bc.itemValue)
                .AddValue(expectedOrderNumber.ToString(), (bookingCode bc) => bc.primaryOrderNumber)
                .AddGenerator(() => (index++).ToString(), (bookingCode bc) => bc.itemNumberWithinBooking);

            var generator = new Helpers.StubGeneration.Generator<order>()
                .AddDefaultGenerators()
                .AddGenerator(() =>
                {
                    index = 1;
                    return bookingCodesGenerator.Generate();
                })
                .AddValue(expectedOrderNumber.ToString(), (order o) => o.primaryOrderNumber)
                .AddValue(expectedOrderNumber.ToString(), (booking b) => b.primaryOrderNumber)
                .AddValue(expectedOrderNumber.ToString(), (resource r) => r.primaryOrderNumber)
                .AddValue(orderStatus.PLANNED, (order o) => o.status)
                .Ignore((order o) => o.phone)
                .Ignore((booking b) => b.theOrder);

            var drsOrder = generator.Generate();
            drsOrder.orderCommentsExtended = _locationAlerts.Alerts.ToCommentsExtendedString() + _personAlerts.Alerts.ToCommentsExtendedString();
            return drsOrder;
        }

        [Test]
        public async Task MapsAlerts()
        {
            var workOrder = _workOrderGenerator.Generate();
            _sorPriorityGatewayMock.Setup(m => m.GetLegacyPriorityCode(It.IsAny<int>())).ReturnsAsync('I');
            var sorCodes = SetupSorCodes(workOrder);

            var request = await _classUnderTest.BuildCreateOrderRequest(_sessionId, workOrder);

            _alertsGatewayMock.Verify(x => x.GetLocationAlertsAsync(workOrder.Site.PropertyClass.First().PropertyReference));
            _alertsGatewayMock.Verify(x => x.GetPersonAlertsAsync(_tenureInformation.TenancyAgreementReference));
        }

        private IList<ScheduleOfRatesModel> SetupSorCodes(WorkOrder workOrder)
        {
            var sorCodes = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem.Select(rsi => new { rsi, we }))
                .Select(x => new ScheduleOfRatesModel
                {
                    Code = x.rsi.CustomCode,
                    ShortDescription = $"{x.rsi.CustomCode} short description",
                    LongDescription = $"{x.rsi.CustomCode} long description",
                    TradeCode = x.we.Trade.FirstOrDefault().CustomCode
                }).ToList();

            if (sorCodes == null) return null;

            foreach (var code in sorCodes)
            {
                _sorGatewayMock.Setup(x => x.GetCode(code.Code, It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(code);
            }

            return sorCodes;
        }

        private List<ScheduleOfRatesModel> SetupSORCodes(order drsOrder)
        {

            var sorCodes = drsOrder.theBookings.SelectMany(b => b.theBookingCodes).Select(c => new ScheduleOfRatesModel
            {
                Code = c.bookingCodeSORCode,
                LongDescription = c.bookingCodeDescription,
                ShortDescription = c.bookingCodeDescription,
                TradeCode = c.trade,
                StandardMinuteValue = int.Parse(c.standardMinuteValue),
                Cost = float.Parse(c.itemValue)
            }).ToList();

            foreach (var code in sorCodes)
            {
                _sorGatewayMock.Setup(x => x.GetCode(code.Code, It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(code);
            }
            return sorCodes;
        }

        private static void VerifyCompleteOrder(updateBooking updateBooking, WorkOrder workOrder, IList<ScheduleOfRatesModel> sorCodes, order drsOrder)
        {
            var booking = drsOrder.theBookings.First();
            updateBooking.updateBooking1.completeOrder.Should().BeTrue();
            updateBooking.updateBooking1.startDateAndTime.Should().Be(booking.assignedStart);
            updateBooking.updateBooking1.endDateAndTime.Should().Be(booking.assignedEnd);
            updateBooking.updateBooking1.resourceId.Should().Be(booking.theResources.First().resourceID);
            updateBooking.updateBooking1.transactionType.Should().Be(transactionTypeType.COMPLETED);

            ValidateBooking(updateBooking.updateBooking1.theBooking, booking);
            ValidateBookings(workOrder, sorCodes, updateBooking.updateBooking1.theBooking.theBookingCodes);

            updateBooking.updateBooking1.theBooking.theOrder.theBookings.Should().BeNull();
            ValidateBusinessData(updateBooking.updateBooking1.theBooking.theOrder.theBusinessData, DrsBusinessDataNames.Status, "COMPLETED");
            updateBooking.updateBooking1.theBooking.theOrder.Should().BeEquivalentTo(drsOrder, c => c
                .Excluding(o => o.status)
                .Excluding(o => o.theBookings)
                .Excluding(o => o.theBusinessData)
            );
            updateBooking.updateBooking1.theBooking.theOrder.status.Should().Be(orderStatus.COMPLETED);
        }

        private void VerifyCreateOrder(createOrder createOrder, WorkOrder workOrder, IList<ScheduleOfRatesModel> sorCodes)
        {
            createOrder.createOrder1.sessionId.Should().Be(_sessionId);
            ValidateOrder(workOrder, createOrder.createOrder1.theOrder, _locationAlerts, _personAlerts);
            ValidateBookings(workOrder, sorCodes, createOrder.createOrder1.theOrder.theBookingCodes);
        }

        private void VerifyDeleteOrder(deleteOrder deleteOrder, WorkOrder workOrder, IList<ScheduleOfRatesModel> sorCodes)
        {
            deleteOrder.deleteOrder1.sessionId.Should().Be(_sessionId);
            ValidateOrder(workOrder, deleteOrder.deleteOrder1.theOrder, _locationAlerts, _personAlerts);
            ValidateBookings(workOrder, sorCodes, deleteOrder.deleteOrder1.theOrder.theBookingCodes);
        }

        private static void ValidateOrder(WorkOrder workOrder, order order, PropertyAlertList locationAlerts, PersonAlertList personAlertList, orderStatus? expectedStatus = null)
        {
            order.primaryOrderNumber.Should().Be(workOrder.Id.ToString(CultureInfo.InvariantCulture));
            order.status.Should().Be(expectedStatus ?? orderStatus.PLANNED);

            var uniqueCodes = locationAlerts?.Alerts.Union(personAlertList?.Alerts);
            order.orderComments.Should().Be(
                @$"{uniqueCodes.ToCodeString()}
                {workOrder.DescriptionOfWork}".Truncate(250));

            order.contract.Should().Be(workOrder.AssignedToPrimary.ContractorReference);
            order.locationID.Should().Be(workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference);
            order.userId.Should().Be(workOrder.AgentEmail);
            order.contactName.Should().Be(workOrder.Customer.Name);
            order.phone.Should().Be(workOrder.Customer.Person.Communication.GetPhoneNumber());

            order.orderCommentsExtended.Should().Contain(locationAlerts.Alerts.ToCommentsExtendedString());
            order.orderCommentsExtended.Should().Contain(personAlertList.Alerts.ToCommentsExtendedString());

            ValidateLocation(workOrder, order.theLocation);
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
            var rateScheduleItems = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem);

            foreach (var (booking, index) in bookings.WithIndex())
            {
                var sorCode = sorCodes.Single(c => c.Code == booking.bookingCodeSORCode);
                var rateScheduleItem = rateScheduleItems.Single(rsi => rsi.CustomCode == booking.bookingCodeSORCode);
                ValidateBookingCode(
                    workOrder,
                    rateScheduleItem,
                    sorCode,
                    booking,
                    index
                );
            }
        }

        private static void ValidateBookingCode(WorkOrder workOrder, RateScheduleItem rateScheduleItem, ScheduleOfRatesModel sorCode, bookingCode booking, int index)
        {
            booking.primaryOrderNumber.Should().Be(workOrder.Id.ToString(CultureInfo.InvariantCulture));
            booking.quantity.Should().Be(rateScheduleItem.Quantity.Amount.ToString(CultureInfo.InvariantCulture));
            booking.bookingCodeSORCode.Should().Be(sorCode.Code);
            booking.bookingCodeDescription.Should().Be(sorCode.LongDescription ?? sorCode.ShortDescription);
            booking.itemValue.Should().BeNull();
            booking.itemNumberWithinBooking.Should().Be((index + 1).ToString(CultureInfo.InvariantCulture));
            index.Should().NotBe(-1);
            booking.trade.Should().Be(sorCode.TradeCode);
            booking.standardMinuteValue.Should().Be(sorCode.StandardMinuteValue.ToString());
        }

        private static void ValidateBooking(booking booking, booking drsBooking)
        {
            booking.bookingCompletionStatus.Should().Be(DrsBookingStatusCodes.Completed);
            booking.bookingLifeCycleStatus.Should().Be(transactionTypeType.COMPLETED);
            ValidateBusinessData(booking.theBusinessData, DrsBusinessDataNames.TaskLifeCycleStatus, DrsTaskLifeCycleCodes.Completed);
            booking.Should().BeEquivalentTo(drsBooking, config => config
                .Excluding(b => b.theOrder)
                .Excluding(b => b.theBookingCodes)
                .Excluding(b => b.bookingLifeCycleStatus)
                .Excluding(b => b.bookingCompletionStatus)
                .Excluding(b => b.theBusinessData));
        }

        private static void ValidateBusinessData(businessData[] newBusinessData, string name, string expectedValue)
        {
            newBusinessData.Single(bd => bd.name == name).value.Should().Be(expectedValue);
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
