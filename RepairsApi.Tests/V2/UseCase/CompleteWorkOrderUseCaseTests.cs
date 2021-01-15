using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Enums;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using Generated = RepairsApi.V2.Generated;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    public class CompleteWorkOrderUseCaseTests
    {
        private Mock<IRepairsGateway> _repairsGatewayMock;
        private CompleteWorkOrderUseCase _classUnderTest;
        private Generator<WorkOrder> _generator;
        private MockWorkOrderCompletionGateway _workOrderCompletionGatewayMock;

        [SetUp]
        public void Setup()
        {
            configureGenerator();
            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _workOrderCompletionGatewayMock = new MockWorkOrderCompletionGateway();
            _classUnderTest = new CompleteWorkOrderUseCase(_repairsGatewayMock.Object, _workOrderCompletionGatewayMock.Object);
        }

        private void configureGenerator()
        {
            _generator = new Generator<WorkOrder>(new Dictionary<Type, IGenerator>
            {
                {
                    typeof(string), new RandomStringGenerator(10)
                },
                {
                    typeof(double), new RandomDoubleGenerator(0, 50)
                },
                {
                    typeof(bool), new RandomBoolGenerator()
                }
            });
        }

        [Test]
        public async Task CanCompleteWorkOrder()
        {
            // arrange
            var expectedWorkOrder = _generator.Generate();
            _repairsGatewayMock.Setup(r => r.GetWorkOrder(expectedWorkOrder.Id))
                .ReturnsAsync(expectedWorkOrder);
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);

            // act
            var result = await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            result.Should().BeTrue();

            var lastWorkOrderComplete = _workOrderCompletionGatewayMock.LastWorkOrderComplete;
            lastWorkOrderComplete.Should().NotBeNull();
            lastWorkOrderComplete.WorkOrder.Should().BeEquivalentTo(expectedWorkOrder);
        }

        [Test]
        public async Task CanCompleteWorkOrderWithJobStatusUpdates()
        {
            // arrange
            var expectedWorkOrder = _generator.Generate();
            _repairsGatewayMock.Setup(r => r.GetWorkOrder(expectedWorkOrder.Id))
                .ReturnsAsync(expectedWorkOrder);
            var workOrderCompleteRequest = CreateRequest(expectedWorkOrder.Id);
            workOrderCompleteRequest.JobStatusUpdates = new List<Generated.JobStatusUpdates>
            {
                new Generated.JobStatusUpdates
                {
                    TypeCode = Generated.JobStatusUpdateTypeCode._0, OtherType = "expectedOtherType", Comments = "expectedComment"
                }
            };

            // act
            var result = await _classUnderTest.Execute(workOrderCompleteRequest);

            // assert
            result.Should().BeTrue();

            var lastWorkOrderComplete = _workOrderCompletionGatewayMock.LastWorkOrderComplete;
            lastWorkOrderComplete.Should().NotBeNull();
            lastWorkOrderComplete.JobStatusUpdates.Should().HaveCount(workOrderCompleteRequest.JobStatusUpdates.Count);
        }

        [Test]
        public async Task ReturnFalseWhenWorkOrderDoesntExist()
        {
            // arrange
            var expectedWorkOrderId = 5;

            // act
            var result = await _classUnderTest.Execute(CreateRequest(expectedWorkOrderId));

            // assert
            result.Should().BeFalse();
        }

        private static Generated.WorkOrderComplete CreateRequest(int expectedWorkOrderId)
        {
            var request = new Generated.WorkOrderComplete
            {
                WorkOrderReference = new Generated.Reference
                {
                    ID = expectedWorkOrderId.ToString()
                }
            };
            return request;
        }
    }

    public class MockWorkOrderCompletionGateway : Mock<IWorkOrderCompletionGateway>
    {
        public WorkOrderComplete LastWorkOrderComplete { get; set; }

        public MockWorkOrderCompletionGateway()
        {
            Setup(g => g.CreateWorkOrderCompletion(It.IsAny<WorkOrderComplete>()))
                .ReturnsAsync(1)
                .Callback<WorkOrderComplete>(woc => LastWorkOrderComplete = woc);
        }
    }
}
