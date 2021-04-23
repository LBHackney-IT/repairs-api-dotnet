using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.MiddleWare;
using RepairsApi.V2.Services;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2;
using RepairsApi.V2.Generated;
using V2_Generated_DRS;
using Trade = RepairsApi.V2.Infrastructure.Trade;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

namespace RepairsApi.Tests.V2.UseCase
{
    public class CreateWorkOrderUseCaseTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesGateway;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IDrsService> _drsServiceMock;
        private CreateWorkOrderUseCase _classUnderTest;
        private Mock<IFeatureManager> _featureManager;
        private IOptions<DrsOptions> _drsOptions;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new MockRepairsGateway();
            _scheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _drsServiceMock = new Mock<IDrsService>();
            _featureManager = new Mock<IFeatureManager>();
            _drsOptions = Options.Create<DrsOptions>(new DrsOptions
            {
                Login = "login",
                Password = "password",
                APIAddress = new Uri("https://apiAddress.none"),
                ManagementAddress = new Uri("https://managementAddress.none")
            });
            _classUnderTest = new CreateWorkOrderUseCase(
                _repairsGatewayMock.Object,
                _scheduleOfRatesGateway.Object,
                new NullLogger<CreateWorkOrderUseCase>(),
                _currentUserServiceMock.Object,
                _drsServiceMock.Object,
                _featureManager.Object,
                _drsOptions
                );
        }

        [Test]
        public async Task Runs()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            var result = await _classUnderTest.Execute(new WorkOrder());

            result.Id.Should().Be(newId);
        }

        [Test]
        public async Task SetsCurrentDate()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            var result = await _classUnderTest.Execute(new WorkOrder());

            VerifyRaiseRepairIsCloseToNow();
        }

        [Test]
        public async Task DoesNotThrowsNotSupportedWhenSingleTradesPosted()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            var workOrder = new WorkOrder
            {
                WorkElements = new List<WorkElement>
                {
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = "trade"}}},
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = "trade"}}}
                }
            };

            var result = await _classUnderTest.Execute(workOrder);

            result.Id.Should().Be(newId);
        }

        [Test]
        public void ThrowsNotSupportedWhenMultipleTradesPosted()
        {
            int newId = 1;
            _repairsGatewayMock.ReturnWOId(newId);
            var workOrder = new WorkOrder
            {
                WorkElements = new List<WorkElement>
                {
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = Guid.NewGuid().ToString()}}},
                    new WorkElement{Trade = new List<Trade>{new Trade{CustomCode = Guid.NewGuid().ToString()}}}
                }
            };

            Assert.ThrowsAsync<NotSupportedException>(async () => await _classUnderTest.Execute(workOrder));
        }

        [Test]
        public async Task SetsRSIToOriginalTrue()
        {
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade> { new Trade { Code = TradeCode.B2 } }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            await _classUnderTest.Execute(workOrder);

            _repairsGatewayMock.LastWorkOrder.WorkElements.All(we => we.RateScheduleItem.All(rsi => rsi.Original))
                .Should().BeTrue();
        }

        [Test]
        public async Task CreatesDRSOrder()
        {
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.DRSIntegration))
                .ReturnsAsync(true);
            _drsServiceMock.Setup(x => x.ContractorUsingDrs(It.IsAny<string>())).ReturnsAsync(true);
            var expectedOrder = CreateExpectedOrder();
            _drsServiceMock.Setup(x => x.CreateOrder(It.IsAny<WorkOrder>())).ReturnsAsync(expectedOrder);
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade> { new Trade { Code = TradeCode.B2 } }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            var result = await _classUnderTest.Execute(workOrder);

            _drsServiceMock.Verify(x => x.CreateOrder(workOrder));
            result.ExternallyManagedAppointment.Should().BeTrue();
            result.ExternalAppointmentManagementUrl.Query.Should().Contain($"tokenId={expectedOrder.theBookings.Single().tokenId}");
        }

        private static order CreateExpectedOrder()
        {
            order expectedOrder = new order
            {
                theBookings = new[]
                {
                    new booking
                    {
                        tokenId = Guid.NewGuid().ToString()
                    }
                }
            };
            return expectedOrder;
        }

        [Test]
        public async Task DoesNotCreateDRSOrder_When_ContractorNotUsingDRS()
        {
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.DRSIntegration))
                .ReturnsAsync(true);
            _drsServiceMock.Setup(x => x.ContractorUsingDrs(It.IsAny<string>())).ReturnsAsync(false);
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade> { new Trade { Code = TradeCode.B2 } }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            var result = await _classUnderTest.Execute(workOrder);

            _drsServiceMock.Verify(x => x.CreateOrder(It.IsAny<WorkOrder>()), Times.Never);
            result.ExternallyManagedAppointment.Should().BeFalse();
        }

        [Test]
        public async Task DoesNotCreateDRSOrder_When_FeatureFlagFalse()
        {
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.DRSIntegration))
                .ReturnsAsync(false);
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade> { new Trade { Code = TradeCode.B2 } }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            await _classUnderTest.Execute(workOrder);

            _drsServiceMock.Verify(x => x.CreateOrder(workOrder), Times.Never);
        }

        private void VerifyRaiseRepairIsCloseToNow()
        {
            _repairsGatewayMock.Verify(m => m.CreateWorkOrder(It.Is<WorkOrder>(wo => AreDatesClose(DateTime.UtcNow, wo.DateRaised.Value, 60000))));
        }

        private static bool AreDatesClose(DateTime d1, DateTime d2, int ms = 60000)
        {
            return Math.Abs((d1 - d2).TotalMilliseconds) < ms;
        }
    }
}
