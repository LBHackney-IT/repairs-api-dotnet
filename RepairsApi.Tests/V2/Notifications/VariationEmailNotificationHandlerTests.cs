using Microsoft.Extensions.Logging.Abstractions;
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
    public class VariationEmailNotificationHandlerTests
    {
        private Mock<IEmailService> _emailMock;
        private FeatureManagerMock _featureManagerMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesMock;
        private VariationEmailNotificationHandler _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _emailMock = new Mock<IEmailService>();
            _featureManagerMock = new FeatureManagerMock();
            _featureManagerMock.SetFeature(FeatureFlags.NotifyIntegration, true);
            _scheduleOfRatesMock = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new VariationEmailNotificationHandler(
                new Lazy<IEmailService>(_emailMock.Object),
                _featureManagerMock.Object,
                new NullLogger<VariationEmailNotificationHandler>(), _scheduleOfRatesMock.Object);
        }

        [Test]
        public async Task HandleVariationReject()
        {
            var variation = new JobStatusUpdate()
            {
                AuthorEmail = "email"
            };
            var rejection = new JobStatusUpdate()
            {
                RelatedWorkOrder = new WorkOrder
                {
                    Id = 1
                }
            };
            await _classUnderTest.Notify(new VariationRejected(variation, rejection));

            _emailMock.Verify(m => m.SendMailAsync(It.Is<VariationRejectedEmail>(email => email.Address == variation.AuthorEmail)));
        }

        [Test]
        public async Task HandleVariationApprove()
        {
            var variation = new JobStatusUpdate()
            {
                AuthorEmail = "email"
            };
            var rejection = new JobStatusUpdate()
            {
                RelatedWorkOrder = new WorkOrder
                {
                    Id = 1
                }
            };
            await _classUnderTest.Notify(new VariationApproved(variation, rejection));

            _emailMock.Verify(m => m.SendMailAsync(It.Is<VariationApprovedEmail>(email => email.Address == variation.AuthorEmail)));
        }

        [Test]
        public async Task HandleHighCostVariation()
        {
            string ExpectedEmail = "email";
            string Contractor = "contractor";
            _scheduleOfRatesMock.Setup(s => s.GetContractManagerEmail(Contractor)).ReturnsAsync(ExpectedEmail);

            var workOrder = new WorkOrder()
            {
                Id = 1,
                AssignedToPrimary = new Party
                {
                    ContractorReference = Contractor
                }
            };
            await _classUnderTest.Notify(new HighCostVariationCreated(workOrder));

            _emailMock.Verify(m => m.SendMailAsync(It.Is<HighCostVariationCreatedEmail>(email => email.Address == ExpectedEmail)));
        }
    }
}
