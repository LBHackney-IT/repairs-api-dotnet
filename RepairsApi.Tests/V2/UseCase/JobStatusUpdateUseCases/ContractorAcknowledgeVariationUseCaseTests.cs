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
    public class ContractorAcknowledgeVariationUseCaseTests
    {
        private Fixture _fixture;

        private MockRepairsGateway _repairsGatewayMock;
        private CurrentUserServiceMock _currentUserServiceMock;
        private ContractorAcknowledgeVariationUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repairsGatewayMock = new MockRepairsGateway();
            _currentUserServiceMock = new CurrentUserServiceMock();
            _classUnderTest = new ContractorAcknowledgeVariationUseCase(
                _repairsGatewayMock.Object,
                _currentUserServiceMock.Object);
        }

        [Test]
        public async Task ThrowAccessExceptionWhenUnauthorizedGroup()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10010);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            await fn.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task ThrowNotSupportedExceptionWhenWorkOrderNotApproved()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId, WorkStatusCode.Open);
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10010);

            _currentUserServiceMock.Setup(currentUser => currentUser.HasGroup(UserGroups.Contractor))
                .Returns(true);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            await fn.Should().ThrowAsync<NotSupportedException>();
        }

        [Test]
        public async Task UpdatesWorkOrderStatus()
        {
            _currentUserServiceMock.SetSecurityGroup(UserGroups.Contractor);
            const int desiredWorkOrderId = 42;
            var workOrder = CreateReturnWorkOrder(desiredWorkOrderId);
            var request = CreateJobStatusUpdateRequest(workOrder,
                Generated.JobStatusUpdateTypeCode._10010);

            await _classUnderTest.Execute(request);

            workOrder.StatusCode.Should().Be(WorkStatusCode.Open);
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
        private WorkOrder CreateReturnWorkOrder(int expectedId, WorkStatusCode workStatusCode = WorkStatusCode.VariationApproved)
        {
            var workOrder = BuildWorkOrder(expectedId, workStatusCode);

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == expectedId)))
                .ReturnsAsync(workOrder);
            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == expectedId)))
                .ReturnsAsync(_fixture.Create<List<WorkElement>>);

            return workOrder;
        }

        private WorkOrder BuildWorkOrder(int expectedId, WorkStatusCode workStatusCode)
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
                .With(x => x.StatusCode, workStatusCode)
                .Create();
            return workOrder;
        }
    }
}
