using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2;
using RepairsApi.V2.Email;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Notifications;
using System;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Notifications
{
    public class WorkOrderEmailNotificationHandlerTests
    {
        private Mock<IEmailService> _emailMock;
        private FeatureManagerMock _featureManagerMock;
        private EmailOptions _options;
        private WorkOrderEmailNotificationHandler _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _emailMock = new Mock<IEmailService>();
            _featureManagerMock = new FeatureManagerMock();
            _featureManagerMock.SetFeature(FeatureFlags.NotifyIntegration, true);

            _options = new EmailOptions
            {
                PendingWorkOrderRecipient = "testEmail"
            };

            _classUnderTest = new WorkOrderEmailNotificationHandler(
                new Lazy<IEmailService>(_emailMock.Object),
                _featureManagerMock.Object,
                new NullLogger<WorkOrderEmailNotificationHandler>(), Options.Create(_options));
        }

        [Test]
        public async Task HandleWorkOrderReject()
        {
            var workOrder = new WorkOrder
            {
                AgentEmail = "testEmail"
            };
            await _classUnderTest.Notify(new WorkOrderRejected(workOrder));

            _emailMock.Verify(m => m.SendMailAsync(It.Is<WorkRejectedEmail>(email => email.Address == workOrder.AgentEmail)));
        }

        [Test]
        public async Task HandleWorkOrderApprove()
        {
            var workOrder = new WorkOrder
            {
                AgentEmail = "testEmail"
            };
            await _classUnderTest.Notify(new WorkOrderApproved(workOrder));

            _emailMock.Verify(m => m.SendMailAsync(It.Is<WorkApprovedEmail>(email => email.Address == workOrder.AgentEmail)));
        }

        [Test]
        public async Task HandleHighCostWorkOrder()
        {
            await _classUnderTest.Notify(new HighCostWorkOrderCreated(new WorkOrder()));

            _emailMock.Verify(m => m.SendMailAsync(It.Is<HighCostWorkOrderEmail>(email => email.Address == _options.PendingWorkOrderRecipient)));
        }
    }
}
