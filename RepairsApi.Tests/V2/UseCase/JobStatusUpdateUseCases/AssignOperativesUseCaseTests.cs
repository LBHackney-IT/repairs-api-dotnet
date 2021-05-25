using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

namespace RepairsApi.Tests.V2.UseCase.JobStatusUpdateUseCases
{
    public class AssignOperativesUseCaseTests
    {
        private AssignOperativesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _classUnderTest = new AssignOperativesUseCase();
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
            (await fn.Should().ThrowAsync<InvalidOperationException>())
                .Which.Message.Should().Be(Resources.ActionUnsupported);
        }

        [Test]
        public async Task SetsAssignedOperatives()
        {
            const int desiredWorkOrderId = 42;
            var workOrder = BuildWorkOrder(desiredWorkOrderId);
            workOrder.StatusCode = WorkStatusCode.Scheduled;
            var request = BuildUpdate(workOrder);

            await _classUnderTest.Execute(request);

            workOrder.AssignedOperatives.Should().BeEquivalentTo(request.OperativesAssigned);
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
                .Without(x => x.AssignedOperatives)
                .Create();
            return workOrder;
        }

        private JobStatusUpdate BuildUpdate(WorkOrder workOrder)
        {

            return _fixture.Build<JobStatusUpdate>()
                .With(jsu => jsu.MoreSpecificSORCode, _fixture.Build<WorkElement>()
                    .With(we => we.RateScheduleItem, _fixture.Build<RateScheduleItem>()
                        .With(rsi => rsi.Quantity, _fixture.Build<Quantity>()
                            .With(q => q.Amount, 1)
                            .Create())
                        .CreateMany().ToList())
                    .Create())
                .With(jsu => jsu.RelatedWorkOrder, workOrder)
                .Create();
        }
    }
}
