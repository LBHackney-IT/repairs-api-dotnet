using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Moq;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using Generated = RepairsApi.V2.Generated;
using RepairsApi.V2.Exceptions;

namespace RepairsApi.Tests.V2.UseCase
{
    public class UpdateJobStatusUseCaseTests
    {
        private Fixture _fixture;

        private MockRepairsGateway _repairsGatewayMock;
        private UpdateJobStatusUseCase _classUnderTest;
        private Mock<IJobStatusUpdateGateway> _jobStatusUpdateGateway;
        private Mock<IJobStatusUpdateStrategyFactory> _strategyFactory;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _repairsGatewayMock = new MockRepairsGateway();
            _jobStatusUpdateGateway = new Mock<IJobStatusUpdateGateway>();
            _strategyFactory = new Mock<IJobStatusUpdateStrategyFactory>();
            _classUnderTest = new UpdateJobStatusUseCase(
                _repairsGatewayMock.Object,
                _jobStatusUpdateGateway.Object,
                _strategyFactory.Object
            );
        }

        [Test]
        public void CanUpdateJobStatus()
        {
            const int desiredWorkOrderId = 42;
            _jobStatusUpdateGateway.Setup(gateway => gateway.CreateJobStatusUpdate(It.Is<JobStatusUpdate>(jsu => jsu.RelatedWorkOrder.Id == desiredWorkOrderId)))
                .ReturnsAsync(1);

            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.Id = desiredWorkOrderId;

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == desiredWorkOrderId)))
                .ReturnsAsync(workOrder);

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == desiredWorkOrderId)))
                .ReturnsAsync(_fixture.Create<List<WorkElement>>);

            Assert.DoesNotThrowAsync(async () => await _classUnderTest.Execute(
                new Generated.JobStatusUpdate
                {
                    RelatedWorkOrderReference = new Generated.Reference
                    {
                        ID = "42"
                    },
                    TypeCode = Generated.JobStatusUpdateTypeCode._0
                }));
        }

        [Test]
        public void DoesNotThrowUnsupportedWhenOtherTypeCode()
        {
            Assert.DoesNotThrowAsync(async () => await _classUnderTest.Execute(
                new Generated.JobStatusUpdate
                {
                    RelatedWorkOrderReference = new Generated.Reference
                    {
                        ID = "41"
                    },
                    TypeCode = Generated.JobStatusUpdateTypeCode._0
                })
            );
        }

        [Test]
        public async Task CanProvideMoreSpecificSorCode()
        {

            const int desiredWorkOrderId = 42;
            _jobStatusUpdateGateway.Setup(gateway => gateway.CreateJobStatusUpdate(It.Is<JobStatusUpdate>(jsu => jsu.RelatedWorkOrder.Id == desiredWorkOrderId)))
                .ReturnsAsync(1);

            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.Id = desiredWorkOrderId;

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == desiredWorkOrderId)))
                .ReturnsAsync(workOrder);

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == desiredWorkOrderId)))
                .ReturnsAsync(_fixture.Create<List<WorkElement>>);

            await _classUnderTest.Execute(CreateMoreSpecificSORUpdateRequest(desiredWorkOrderId, workOrder, "code"));

            _strategyFactory.Verify(uc => uc.ProcessActions(It.IsAny<JobStatusUpdate>()));
        }

        private static Generated.JobStatusUpdate CreateMoreSpecificSORUpdateRequest(int desiredWorkOrderId, WorkOrder workOrder, string expectedNewCode)
        {

            return new Generated.JobStatusUpdate
            {
                RelatedWorkOrderReference = new Generated.Reference
                {
                    ID = desiredWorkOrderId.ToString()
                },
                TypeCode = Generated.JobStatusUpdateTypeCode._80,
                MoreSpecificSORCode = new Generated.WorkElement
                {
                    Trade = new List<Generated.Trade>
                    {
                        new Generated.Trade
                        {
                            CustomCode = workOrder.WorkElements.First().Trade.First().CustomCode
                        }
                    },
                    RateScheduleItem = new List<Generated.RateScheduleItem>
                    {
                        new Generated.RateScheduleItem
                        {
                            CustomCode = expectedNewCode
                        }
                    }
                }
            };
        }
    }
}
