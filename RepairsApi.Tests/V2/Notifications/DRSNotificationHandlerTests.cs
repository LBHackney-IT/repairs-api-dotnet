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

namespace RepairsApi.Tests.V2.Notifications
{
    public class DRSNotificationHandlerTests
    {
        private DRSNotificationHandler _classUnderTest;
        private Mock<IFeatureManager> _featureManager;
        private Mock<IDrsService> _drsServiceMock;

        [SetUp]
        public void Setup()
        {
            _featureManager = new Mock<IFeatureManager>();
            _drsServiceMock = new Mock<IDrsService>();

            ServiceCollection services = new ServiceCollection();
            services.AddTransient(sp => _drsServiceMock.Object);
            _classUnderTest = new DRSNotificationHandler(services.BuildServiceProvider(), _featureManager.Object);
        }

        [Test]
        public async Task CreatesDRSOrder()
        {
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.DRSIntegration))
                .ReturnsAsync(true);
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade> { new Trade { Code = RepairsApi.V2.Generated.TradeCode.B2 } }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            await _classUnderTest.Notify(new WorkOrderOpened(workOrder));

            _drsServiceMock.Verify(x => x.CreateOrder(workOrder));
        }

        [Test]
        public async Task DoesNotCreateDRSOrder_When_FeatureFlagFalse()
        {
            _featureManager.Setup(x => x.IsEnabledAsync(FeatureFlags.DRSIntegration))
                .ReturnsAsync(false);
            var generator = new Generator<WorkOrder>()
                .AddInfrastructureWorkOrderGenerators()
                .AddValue(new List<Trade> { new Trade { Code = RepairsApi.V2.Generated.TradeCode.B2 } }, (WorkElement we) => we.Trade);
            var workOrder = generator.Generate();

            await _classUnderTest.Notify(new WorkOrderOpened(workOrder));

            _drsServiceMock.Verify(x => x.CreateOrder(workOrder), Times.Never);
        }
    }
}
