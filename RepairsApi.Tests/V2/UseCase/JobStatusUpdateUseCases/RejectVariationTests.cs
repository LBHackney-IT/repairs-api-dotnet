using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2;
using Generated = RepairsApi.V2.Generated;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;
using RepairsApi.V2.Notifications;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class RejectVariationTests
    {
        private Fixture _fixture;

        private MockRepairsGateway _repairsGatewayMock;
        private CurrentUserServiceMock _currentUserServiceMock;
        private Mock<IJobStatusUpdateGateway> _jobStatusUpdateGatewayMock;
        private NotificationMock _notifier;
        private RejectVariationUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repairsGatewayMock = new MockRepairsGateway();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _jobStatusUpdateGatewayMock = new Mock<IJobStatusUpdateGateway>();
            _notifier = new NotificationMock();
            _classUnderTest = new RejectVariationUseCase(
                _repairsGatewayMock.Object,
                _currentUserServiceMock.Object,
                _jobStatusUpdateGatewayMock.Object,
                _notifier);
        }

        private static readonly IEnumerable<string> _testGroups = EnumerationHelper.GetStaticValues(typeof(UserGroups), UserGroups.ContractManager);
        [Test, TestCaseSource(nameof(_testGroups))]
        public async Task ThrowsUnauthorizedWhenUserNotInGroup(string userGroup)
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._125);

            _currentUserServiceMock.SetSecurityGroup(userGroup);

            Func<Task> fn = async () => await _classUnderTest.Execute(request);
            (await fn.Should().ThrowAsync<UnauthorizedAccessException>())
                .Which.Message.Should().Be(Resources.InvalidPermissions);

        }

        private static readonly IEnumerable<WorkStatusCode> _testCodes = Enum.GetValues(typeof(WorkStatusCode)).Cast<WorkStatusCode>()
            .Where(c => c != WorkStatusCode.VariationPendingApproval);
        [Test, TestCaseSource(nameof(_testCodes))]
        public async Task ThrowNotSupportedExceptionWhenWorkStatusNotPendingApproval(WorkStatusCode status)
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId, status);
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._125);

            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            await fn.Should().ThrowAsync<NotSupportedException>();
        }

        [Test]
        public async Task SetWorkOrderStatusWhenAuthorizedGroup()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._125);

            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            await _classUnderTest.Execute(request);

            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationRejected);
        }

        [Test]
        public async Task PrependsRejectString()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._125);
            const string beforeComments = "expectedBeforeComments";
            request.Comments = beforeComments;

            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            await _classUnderTest.Execute(request);

            request.Comments.Should().Contain(beforeComments);
            request.Comments.Should().Contain(Resources.RejectedVariationPrepend);
        }

        [Test]
        public async Task DoesntPrependsRejectStringWhenAlreadyPresent()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._125);
            var expectedComments = $"{Resources.RejectedVariationPrepend}expectedBeforeComments";
            request.Comments = expectedComments;

            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            await _classUnderTest.Execute(request);

            request.Comments.Should().Be(expectedComments);
        }

        [Test]
        public async Task SendsRejectedNotification()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._125);

            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            await _classUnderTest.Execute(request);

            _notifier.HaveHandlersBeenCalled<VariationRejected>().Should().BeTrue();
        }

        private static JobStatusUpdate CreateJobStatusUpdateRequest
            (WorkOrder workOrder, Generated.JobStatusUpdateTypeCode jobStatus)
        {
            return new JobStatusUpdate
            {
                RelatedWorkOrder = workOrder,
                TypeCode = jobStatus,
                MoreSpecificSORCode = new WorkElement
                {
                    Trade = new List<Trade>(),
                    RateScheduleItem = new List<RateScheduleItem>()
                }
            };
        }
        private WorkOrder CreateReturnWorkOrder(int expectedId, WorkStatusCode statusCode = WorkStatusCode.VariationPendingApproval)
        {
            var workOrder = BuildWorkOrder(expectedId, statusCode);

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == expectedId)))
                .ReturnsAsync(workOrder);
            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == expectedId)))
                .ReturnsAsync(_fixture.Create<List<WorkElement>>);

            return workOrder;
        }

        private WorkOrder BuildWorkOrder(int expectedId, WorkStatusCode statusCode)
        {
            var workOrder = _fixture.Build<WorkOrder>()
                .With(x => x.WorkElements, new List<WorkElement>
                {
                    _fixture.Build<WorkElement>()
                        .With(x => x.RateScheduleItem,
                            new List<RateScheduleItem>
                            {
                                _fixture.Create<RateScheduleItem>()
                            }
                        ).With(x => x.Trade,
                            new List<Trade>
                            {
                                _fixture.Create<Trade>()
                            })
                        .Create()
                })
                .With(x => x.Id, expectedId)
                .With(x => x.StatusCode, statusCode)
                .Create();
            return workOrder;
        }
    }
}
