using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class JobIncompleteNeedMaterialsStrategyTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private JobIncompleteNeedMaterialsStrategy _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _repairsGatewayMock = new MockRepairsGateway();
            _classUnderTest = new JobIncompleteNeedMaterialsStrategy(_repairsGatewayMock.Object);
        }

        [Test]
        public async Task ThrowsNotSupportedWhenWoInWrongStatus()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.PendingApproval;
            var jobStatusUpdate = BuildUpdate(workOrder);
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);

            Func<Task> act = () => _classUnderTest.Execute(jobStatusUpdate);

            (await act.Should().ThrowAsync<NotSupportedException>())
                .Which.Message.Should().Be(Resources.ActionUnsupported);
        }

        private static IEnumerable<WorkStatusCode> _testCodes = Enum.GetValues(typeof(WorkStatusCode)).Cast<WorkStatusCode>()
            .Where(c => c != WorkStatusCode.PendingApproval);
        [Test, TestCaseSource(nameof(_testCodes))]
        public async Task UpdatesWorkOrderStatus(WorkStatusCode statusCode)
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = statusCode;
            var jobStatusUpdate = BuildUpdate(workOrder);
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);

            await _classUnderTest.Execute(jobStatusUpdate);

            workOrder.StatusCode.Should().Be(WorkStatusCode.PendMaterial);
        }

        [Test]
        public async Task CallsSaveChanges()
        {
            var workOrder = _fixture.Create<WorkOrder>();
            workOrder.StatusCode = WorkStatusCode.Hold;
            var jobStatusUpdate = BuildUpdate(workOrder);
            _repairsGatewayMock.ReturnsWorkOrders(workOrder);

            await _classUnderTest.Execute(jobStatusUpdate);

            _repairsGatewayMock.VerifyChangesSaved();
        }


        private JobStatusUpdate BuildUpdate(WorkOrder workOrder)
        {
            var jobStatusUpdate = _fixture.Build<JobStatusUpdate>()
                .With(jsu => jsu.RelatedWorkOrder, workOrder).Create();
            return jobStatusUpdate;
        }
    }
}
