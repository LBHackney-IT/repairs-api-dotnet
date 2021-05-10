using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class ApproveWorkOrderStrategyTests
    {
        private CurrentUserServiceMock _currentUserServiceMock;
        private MockRepairsGateway _repairsGatewayMock;
        private NotificationMock _notifierMock;
        private ApproveWorkOrderStrategy _classUnderTest;
        private Fixture _fixture;
        private string _expectedName;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _currentUserServiceMock = new CurrentUserServiceMock();
            _repairsGatewayMock = new MockRepairsGateway();
            _notifierMock = new NotificationMock();
            _expectedName = "Expected Name";
            _currentUserServiceMock.SetUser("1111", "expected@email.com", _expectedName);
            _classUnderTest = new ApproveWorkOrderStrategy(
                _currentUserServiceMock.Object,
                _repairsGatewayMock.Object,
                _notifierMock
            );
        }

        private static IEnumerable<WorkStatusCode> _testCodes = Enum.GetValues(typeof(WorkStatusCode)).Cast<WorkStatusCode>()
            .Where(c => c != WorkStatusCode.PendingApproval);
        [Test, TestCaseSource(nameof(_testCodes))]
        public async Task ThrowsNotSupportedWhenWoInWrongState(WorkStatusCode status)
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = status;
            var jobStatusUpdate = BuildUpdate(workOrder);
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);

            Func<Task> act = () => _classUnderTest.Execute(jobStatusUpdate);

            (await act.Should().ThrowAsync<NotSupportedException>())
                .Which.Message.Should().Be(Resources.WorkOrderNotPendingApproval);
        }

        private static IEnumerable<string> _testGroups = EnumerationHelper.GetStaticValues(typeof(UserGroups), UserGroups.AuthorisationManager);
        [Test, TestCaseSource(nameof(_testGroups))]
        public async Task ThrowsUnauthorizedWhenUserNotInGroup(string userGroup)
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            var jobStatusUpdate = BuildUpdate(workOrder);
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            _currentUserServiceMock.SetSecurityGroup(userGroup);

            Func<Task> act = async () => await _classUnderTest.Execute(jobStatusUpdate);

            (await act.Should().ThrowAsync<UnauthorizedAccessException>())
                .Which.Message.Should().Be(Resources.InvalidPermissions);
        }

        [Test]
        public async Task UpdatesWorkOrderStatus()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            var jobStatusUpdate = BuildUpdate(workOrder);
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.AuthorisationManager);

            await _classUnderTest.Execute(jobStatusUpdate);

            workOrder.StatusCode.Should().Be(WorkStatusCode.Open);
        }

        [Test]
        public async Task NotifiesHandlers()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            var jobStatusUpdate = BuildUpdate(workOrder);
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.AuthorisationManager);

            await _classUnderTest.Execute(jobStatusUpdate);

            _notifierMock.HaveHandlersBeenCalled<WorkOrderOpened>().Should().BeTrue();
            _notifierMock.GetLastNotification<WorkOrderOpened>()
                .WorkOrder.Id.Should().Be(workOrder.Id);
        }

        [Test]
        public async Task AppendUserToComments()
        {
            const string beforeComment = "expectedBeforeComment";
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            var jobStatusUpdate = BuildUpdate(workOrder);
            jobStatusUpdate.Comments = beforeComment;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.AuthorisationManager);

            await _classUnderTest.Execute(jobStatusUpdate);

            jobStatusUpdate.Comments.Should().Contain(beforeComment);
            jobStatusUpdate.Comments.Should().Contain(_expectedName);
        }

        private JobStatusUpdate BuildUpdate(WorkOrder workOrder)
        {
            var jobStatusUpdate = _fixture.Build<JobStatusUpdate>()
                .With(jsu => jsu.RelatedWorkOrderReference,
                    _fixture.Build<Reference>()
                        .With(r => r.ID, workOrder.Id.ToString)
                        .Create())
                .Create();
            return jobStatusUpdate;
        }
    }
}
