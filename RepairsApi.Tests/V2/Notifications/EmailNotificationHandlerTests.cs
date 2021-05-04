using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Notifications
{
    public class EmailNotificationHandlerTests
    {
        private Mock<IFeatureManager> _featureManager;
        private MockGovUKNotifyWrapper _serviceMock;
        private EmailNotificationHandler _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _featureManager = new Mock<IFeatureManager>();
            _serviceMock = new MockGovUKNotifyWrapper();

            _classUnderTest = new EmailNotificationHandler(new Lazy<IGovUKNotifyWrapper>(_serviceMock), _featureManager.Object);
        }

        [Test]
        public async Task NotSentIfDisabled()
        {
            _featureManager.Setup(f => f.IsEnabledAsync(It.IsAny<string>())).ReturnsAsync(false);

            await _classUnderTest.Notify(new WorkOrderCompleted(null, null));

            _serviceMock.SentMails.Should().BeEmpty();
        }

        [Test]
        public async Task SentIfEnabled()
        {
            _featureManager.Setup(f => f.IsEnabledAsync(It.IsAny<string>())).ReturnsAsync(true);

            await _classUnderTest.Notify(BuildComplete());

            _serviceMock.SentMails.Should().HaveCount(1);
        }

        private static WorkOrderCompleted BuildComplete()
        {
            return new WorkOrderCompleted(new RepairsApi.V2.Infrastructure.WorkOrder { Id = 1 }, new RepairsApi.V2.Generated.JobStatusUpdates { Comments = "string" });
        }
    }
}
