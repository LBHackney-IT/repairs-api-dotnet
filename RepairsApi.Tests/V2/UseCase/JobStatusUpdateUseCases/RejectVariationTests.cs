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
using System.Threading.Tasks;
using Generated = RepairsApi.V2.Generated;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class RejectVariationTests
    {
        private Fixture _fixture;

        private MockRepairsGateway _repairsGatewayMock;
        private CurrentUserServiceMock _currentUserServiceMock;
        private RejectVariationUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repairsGatewayMock = new MockRepairsGateway();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _classUnderTest = new RejectVariationUseCase(
                _repairsGatewayMock.Object,
                _currentUserServiceMock.Object);
        }

        [Test]
        public void ThrowAccessExceptionWhenUnauthorizedGroup()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var request = CreateJobStatusUpdateRequest(desiredWorkOrderId,
                Generated.JobStatusUpdateTypeCode._125);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            fn.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public void ThrowNotSupportedExceptionWhenWorkStatusNotPendingApproval()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var request = CreateJobStatusUpdateRequest(desiredWorkOrderId,
                Generated.JobStatusUpdateTypeCode._125);

            _currentUserServiceMock.Setup(currentUser => currentUser.HasGroup(UserGroups.CONTRACT_MANAGER))
                .Returns(true);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            fn.Should().ThrowAsync<NotSupportedException>();
        }

        [Test]
        public async Task SetWorkOrderStatusWhenAuthorizedGroup()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
            var request = CreateJobStatusUpdateRequest(desiredWorkOrderId,
                Generated.JobStatusUpdateTypeCode._125);

            _currentUserServiceMock.Setup(currentUser => currentUser.HasGroup(UserGroups.CONTRACT_MANAGER))
                .Returns(true);

            await _classUnderTest.Execute(request);
            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationRejected);
        }

        private static Generated.JobStatusUpdate CreateJobStatusUpdateRequest
            (int workOrderId, Generated.JobStatusUpdateTypeCode jobStatus)
        {
            return new Generated.JobStatusUpdate
            {
                RelatedWorkOrderReference = new Generated.Reference
                {
                    ID = workOrderId.ToString()
                },
                TypeCode = jobStatus,
                MoreSpecificSORCode = new Generated.WorkElement
                {
                    Trade = new List<Generated.Trade>(),
                    RateScheduleItem = new List<Generated.RateScheduleItem>()
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
