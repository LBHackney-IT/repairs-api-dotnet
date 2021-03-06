using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2;
using Generated = RepairsApi.V2.Generated;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;
using RepairsApi.V2.Notifications;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class ApproveVariationTests
    {
        private Fixture _fixture;

        private MockRepairsGateway _repairsGatewayMock;
        private CurrentUserServiceMock _currentUserServiceMock;
        private NotificationMock _notifierMock;
        private AuthorisationMock _authorisationMock;
        private ApproveVariationUseCase _classUnderTest;
        private Mock<IJobStatusUpdateGateway> _jobStatusUpdateGateway;
        private Mock<IUpdateSorCodesUseCase> _updateSorCodesUseCaseMock;
        private string _expectedName;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repairsGatewayMock = new MockRepairsGateway();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _jobStatusUpdateGateway = new Mock<IJobStatusUpdateGateway>();
            _updateSorCodesUseCaseMock = new Mock<IUpdateSorCodesUseCase>();
            _expectedName = "Expected Name";
            _currentUserServiceMock.SetUser("1111", "expected@email.com", _expectedName);
            _notifierMock = new NotificationMock();
            _authorisationMock = new AuthorisationMock();
            _authorisationMock.SetPolicyResult("VarySpendLimit", true);

            _classUnderTest = new ApproveVariationUseCase(
                _repairsGatewayMock.Object, _jobStatusUpdateGateway.Object,
                _currentUserServiceMock.Object, _updateSorCodesUseCaseMock.Object, _notifierMock,
                _authorisationMock.Object);
        }

        [Test]
        public async Task ThrowAccessExceptionWhenUnauthorizedGroup()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10020);

            Func<Task> fn = async () => await _classUnderTest.Execute(request);
            await fn.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task ThrowNotSupportedExceptionWhenWoInWrongState([Values] WorkStatusCode statusCode)
        {
            if (statusCode == WorkStatusCode.VariationPendingApproval) return;

            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = statusCode;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10020);

            Func<Task> fn = async () => await _classUnderTest.Execute(request);
            (await fn.Should().ThrowAsync<NotSupportedException>())
                .Which.Message.Should().Be(Resources.ActionUnsupported);
        }

        [Test]
        public async Task CallsUpdateSorUseCase()
        {
            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10020);

            var expectedJobStatusUpdate = _fixture.Create<JobStatusUpdate>();
            _jobStatusUpdateGateway.Setup(x => x.GetOutstandingVariation(desiredWorkOrderId))
                .ReturnsAsync(expectedJobStatusUpdate);

            await _classUnderTest.Execute(request);

            _updateSorCodesUseCaseMock.Verify(x => x.Execute(workOrder, expectedJobStatusUpdate.MoreSpecificSORCode));
            _repairsGatewayMock.VerifyChangesSaved();
        }

        [Test]
        public async Task AppendsUserToComment()
        {
            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10020);
            string beforeComment = "expectedBefore";
            request.Comments = beforeComment;

            var expectedJobStatusUpdate = _fixture.Create<JobStatusUpdate>();
            _jobStatusUpdateGateway.Setup(x => x.GetOutstandingVariation(desiredWorkOrderId))
                .ReturnsAsync(expectedJobStatusUpdate);

            await _classUnderTest.Execute(request);

            request.Comments.Should().Contain(beforeComment);
            request.Comments.Should().Contain(_expectedName);
        }

        [Test]
        public async Task SendsNotification()
        {
            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10020);

            var expectedJobStatusUpdate = _fixture.Create<JobStatusUpdate>();
            _jobStatusUpdateGateway.Setup(x => x.GetOutstandingVariation(desiredWorkOrderId))
                .ReturnsAsync(expectedJobStatusUpdate);

            await _classUnderTest.Execute(request);

            _notifierMock.HaveHandlersBeenCalled<VariationApproved>().Should().BeTrue();
        }

        [Test]
        public async Task ThrowsNotAuthorisedWhenAboveSpendLimit()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10020);
            _currentUserServiceMock.SetSecurityGroup(UserGroups.ContractManager);

            _authorisationMock.SetPolicyResult("VarySpendLimit", false);

            Func<Task> fn = async () => await _classUnderTest.Execute(request);
            await fn.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        private static JobStatusUpdate CreateJobStatusUpdateRequest(WorkOrder workOrder, Generated.JobStatusUpdateTypeCode jobStatus)
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

        private WorkOrder CreateReturnWorkOrder(int expectedId)
        {
            var workOrder = BuildWorkOrder(expectedId);

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == expectedId)))
                .ReturnsAsync(workOrder);
            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == expectedId)))
                .ReturnsAsync(_fixture.Create<List<WorkElement>>);

            return workOrder;
        }

        private WorkOrder BuildWorkOrder(int expectedId)
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
                .Create();
            return workOrder;
        }
    }
}
