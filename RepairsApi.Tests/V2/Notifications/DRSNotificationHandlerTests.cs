using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;

namespace RepairsApi.Tests.V2.Notifications
{
    public class DRSNotificationHandlerTests
    {
        private DRSNotificationHandler _classUnderTest;
        private Mock<IFeatureManager> _featureManager;
        private Mock<IDrsService> _drsServiceMock;
        private Mock<IScheduleOfRatesGateway> _sorGatewayMock;

        [SetUp]
        public void Setup()
        {
            _featureManager = new Mock<IFeatureManager>();
            _drsServiceMock = new Mock<IDrsService>();
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            FeatureEnabled(true);
            ContractorUsesExternalScheduler(true);

            var services = new ServiceCollection();
            services.AddTransient(sp => _drsServiceMock.Object);
            _classUnderTest = new DRSNotificationHandler(
                services.BuildServiceProvider(),
                _featureManager.Object,
                _sorGatewayMock.Object);
        }

        [Test]
        public async Task CreatesDRSOrder()
        {
            var workOrder = CreateWorkOrder();

            await _classUnderTest.Notify(new WorkOrderOpened(workOrder));

            _drsServiceMock.Verify(x => x.CreateOrder(workOrder));
        }

        [Test]
        public async Task DoesNotCreateDRSOrder_When_FeatureFlagFalse()
        {
            FeatureEnabled(false);
            var workOrder = CreateWorkOrder();

            await _classUnderTest.Notify(new WorkOrderOpened(workOrder));

            _drsServiceMock.Verify(x => x.CreateOrder(workOrder), Times.Never);
        }

        [Test]
        public async Task DoesNotCreateDRSOrder_When_ContractorNotEnabled()
        {
            ContractorUsesExternalScheduler(false);
            var workOrder = CreateWorkOrder();

            await _classUnderTest.Notify(new WorkOrderOpened(workOrder));

            _drsServiceMock.Verify(x => x.CreateOrder(workOrder), Times.Never);
        }

        [Test]
        public async Task DeletesDRSOrder()
        {
            var workOrder = CreateWorkOrder();

            await _classUnderTest.Notify(new WorkOrderCancelled(workOrder));

            _drsServiceMock.Verify(x => x.CancelOrder(workOrder));
        }

        [Test]
        public async Task DoesNotDeleteDRSOrder_When_FeatureFlagFalse()
        {
            FeatureEnabled(false);
            var workOrder = CreateWorkOrder();

            await _classUnderTest.Notify(new WorkOrderCancelled(workOrder));

            _drsServiceMock.Verify(x => x.CancelOrder(workOrder), Times.Never);
        }

        [Test]
        public async Task DoesNotDeleteDRSOrder_When_ContractorNotEnabled()
        {
            ContractorUsesExternalScheduler(false);
            var workOrder = CreateWorkOrder();

            await _classUnderTest.Notify(new WorkOrderCancelled(workOrder));

            _drsServiceMock.Verify(x => x.CancelOrder(workOrder), Times.Never);
        }

        private static WorkOrder CreateWorkOrder()
        {

            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade>
                {
                    new Trade
                    {
                        Code = RepairsApi.V2.Generated.TradeCode.B2
                    }
                }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();
            return workOrder;
        }

        private void FeatureEnabled(bool enabled)
        {
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.DRSIntegration))
                .ReturnsAsync(enabled);
        }

        private void ContractorUsesExternalScheduler(bool external)
        {
            _sorGatewayMock.Setup(x => x.GetContractor(It.IsAny<string>()))
                .ReturnsAsync(new Contractor
                {
                    UseExternalScheduleManager = external
                });
        }

    }
}
