using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class AssignOperativesUseCaseTests
    {
        private Mock<IOperativesGateway> _operativesGatewayMock;
        private Mock<IScheduleOfRatesGateway> _scheduleOfRatesGateway;
        private AssignOperativesUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _operativesGatewayMock = new Mock<IOperativesGateway>();
            _scheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            _scheduleOfRatesGateway.Setup(mock => mock.GetContractor(It.IsAny<string>())).ReturnsAsync(new RepairsApi.V2.Domain.Contractor { UseExternalScheduleManager = true });

            _classUnderTest = new AssignOperativesUseCase(_operativesGatewayMock.Object, _scheduleOfRatesGateway.Object);
        }

        [TestCase(WorkStatusCode.VariationPendingApproval)]
        [TestCase(WorkStatusCode.PendingApproval)]
        public async Task ThrowWhenWorkOrderNotInCorrectState(WorkStatusCode state)
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = state;
            var request = BuildUpdate(workOrder);

            Func<Task> fn = () => _classUnderTest.Execute(request);
            (await fn.Should().ThrowAsync<NotSupportedException>())
                .Which.Message.Should().Be(Resources.ActionUnsupported);
        }

        [Test]
        public async Task SetsAssignedOperatives()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Scheduled;
            var request = BuildUpdate(workOrder);
            var expectedAssignments = request.OperativesAssigned.Select(o => new WorkOrderOperative { OperativeId = int.Parse(o.Identification.Number), WorkOrderId = desiredWorkOrderId }).ToArray();

            await _classUnderTest.Execute(request);

            _operativesGatewayMock.Verify(mock => mock.AssignOperatives(It.Is<WorkOrderOperative[]>(assignment => assignment.Length == expectedAssignments.Length)));
        }

        private static WorkOrder BuildWorkOrder(int expectedId)
        {
            return new WorkOrder() { Id = expectedId, AssignedToPrimary = new Party { ContractorReference = "test-contractor" } };
        }

        private static JobStatusUpdate BuildUpdate(WorkOrder workOrder)
        {
            return new Generator<JobStatusUpdate>()
                .AddDefaultGenerators()
                .AddGenerator(() => new Faker().Random.Int(0).ToString(), (Identification i) => i.Number)
                .AddValue(workOrder, (JobStatusUpdate jsu) => jsu.RelatedWorkOrder)
                .Generate();
        }
    }
}
