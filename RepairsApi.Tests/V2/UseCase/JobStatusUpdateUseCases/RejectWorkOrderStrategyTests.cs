using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class RejectWorkOrderStrategyTests
    {
        private CurrentUserServiceMock _currentUserServiceMock;
        private MockRepairsGateway _repairsGatewayMock;
        private RejectWorkOrderStrategy _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _currentUserServiceMock = new CurrentUserServiceMock();
            _repairsGatewayMock = new MockRepairsGateway();
            _classUnderTest = new RejectWorkOrderStrategy(
                _currentUserServiceMock.Object,
                _repairsGatewayMock.Object
            );
        }

        private static IEnumerable<WorkStatusCode> _testCodes = Enum.GetValues(typeof(WorkStatusCode)).Cast<WorkStatusCode>()
            .Where(c => c != WorkStatusCode.PendingApproval);
        [Test, TestCaseSource(nameof(_testCodes))]
        public async Task ThrowsNotSupportedWhenWOInWrongState(WorkStatusCode status)
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = status;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var jobStatusUpdate = BuildUpdate(workOrder);

            Func<Task> act = async () => await _classUnderTest.Execute(jobStatusUpdate);

            (await act.Should().ThrowAsync<NotSupportedException>())
                .Which.Message.Should().Be(Resources.WorkOrderNotPendingApproval);
        }

        [Test, TestCaseSource(typeof(EnumerationHelper), nameof(EnumerationHelper.GetStaticValuesWithExclude), new object[] { typeof(UserGroups), UserGroups.AuthorisationManager })]
        public async Task ThrowsUnauthorizedWhenUserNotInGroup(string userGroup)
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var jobStatusUpdate = BuildUpdate(workOrder);
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
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var jobStatusUpdate = BuildUpdate(workOrder);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.AuthorisationManager);

            await _classUnderTest.Execute(jobStatusUpdate);

            workOrder.StatusCode.Should().Be(WorkStatusCode.Canceled);
        }

        [Test]
        public async Task PrependsRejectString()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var jobStatusUpdate = BuildUpdate(workOrder);
            const string beforeComments = "expectedBeforeComments";
            jobStatusUpdate.Comments = beforeComments;

            _currentUserServiceMock.SetSecurityGroup(UserGroups.AuthorisationManager);

            await _classUnderTest.Execute(jobStatusUpdate);

            jobStatusUpdate.Comments.Should().Contain(beforeComments);
            jobStatusUpdate.Comments.Should().Contain(Resources.WorkOrderAuthorisationRejected);
        }

        [Test]
        public async Task DoesntPrependsRejectStringWhenAlreadyPresent()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);
            var jobStatusUpdate = BuildUpdate(workOrder);
            var expectedComments = $"{Resources.WorkOrderAuthorisationRejected}expectedBeforeComments";
            jobStatusUpdate.Comments = expectedComments;

            _currentUserServiceMock.SetSecurityGroup(UserGroups.AuthorisationManager);

            await _classUnderTest.Execute(jobStatusUpdate);

            jobStatusUpdate.Comments.Should().Be(expectedComments);
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