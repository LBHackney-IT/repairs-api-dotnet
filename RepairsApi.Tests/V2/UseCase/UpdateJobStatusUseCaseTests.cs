using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Moq;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using Generated = RepairsApi.V2.Generated;

namespace RepairsApi.Tests.V2.UseCase
{
    public class UpdateJobStatusUseCaseTests
    {
        private Fixture _fixture;

        private Mock<IRepairsGateway> _repairsGatewayMock;
        private UpdateJobStatusUseCase _classUnderTest;
        private Mock<IJobStatusUpdateGateway> _jobStatusUpdateGateway;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _repairsGatewayMock = new Mock<IRepairsGateway>();
            _jobStatusUpdateGateway = new Mock<IJobStatusUpdateGateway>();
            _classUnderTest = new UpdateJobStatusUseCase(_repairsGatewayMock.Object, _jobStatusUpdateGateway.Object);
        }

        [Test]
        public async Task CanUpdateJobStatus()
        {
            _jobStatusUpdateGateway.Setup(gateway => gateway.CreateJobStatusUpdate(It.Is<JobStatusUpdate>(jsu => jsu.RelatedWorkOrder.Id == 42)))
                .ReturnsAsync(1);

            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.Id = 42;

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkOrder(It.Is<int>(i => i == 42)))
                .Returns(workOrder);

            _repairsGatewayMock.Setup(gateway => gateway.GetWorkElementsForWorkOrder(It.Is<WorkOrder>(wo => wo.Id == 42)))
                .Returns(_fixture.Create<List<WorkElement>>);

            var result = await _classUnderTest.Execute(
                new Generated.JobStatusUpdate {RelatedWorkOrderReference = new Generated.Reference {ID = "42"}});

            result.Should().BeTrue();
        }

        [Test]
        public async Task ReturnFalseWhenWorkOrderDoesntExist()
        {
            var result = await _classUnderTest.Execute(
                new Generated.JobStatusUpdate {RelatedWorkOrderReference = new Generated.Reference {ID = "41"}});

            // assert
            result.Should().BeFalse();
        }
    }
}
