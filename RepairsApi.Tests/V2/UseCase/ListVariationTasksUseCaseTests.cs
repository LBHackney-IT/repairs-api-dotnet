using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.V2.Gateways;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    public class ListVariationTasksUseCaseTests
    {
        private MockRepairsGateway _repairsGatewayMock;
        private Mock<IJobStatusUpdateGateway> _jobStatusUpdateGatewayMock;
        private ListVariationTasksUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _repairsGatewayMock = new MockRepairsGateway();
            _jobStatusUpdateGatewayMock = new Mock<IJobStatusUpdateGateway>();
            _classUnderTest = new ListVariationTasksUseCase(_jobStatusUpdateGatewayMock.Object, _repairsGatewayMock.Object);
        }

        [TestCase(WorkStatusCode.Open)]
        [TestCase(WorkStatusCode.Complete)]
        [TestCase(WorkStatusCode.Canceled)]
        [TestCase(WorkStatusCode.Hold)]
        [TestCase(WorkStatusCode.VariationApproved)]
        [TestCase(WorkStatusCode.VariationRejected)]
        [TestCase(WorkStatusCode.PendMaterial)]
        [TestCase(WorkStatusCode.PendingApproval)]
        public async Task ThrowsNotSupportedIfNotPendingApproval(WorkStatusCode status)
        {
            _repairsGatewayMock.Setup(g => g.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(new WorkOrder { StatusCode = status });

            Func<Task> testFn = async () => await _classUnderTest.Execute(1);

            await testFn.Should().ThrowAsync<NotSupportedException>();
        }

        [Test]
        public async Task GetsData()
        {
            const int originalQuantity = 10;
            const int updatedQuantity = 15;
            const int newQuantity = 5;
            Guid existingItemGuid = Guid.NewGuid();
            _repairsGatewayMock.Setup(g => g.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(BuildWorkOrder(existingItemGuid, originalQuantity));
            _jobStatusUpdateGatewayMock.Setup(g => g.GetOutstandingVariation(It.IsAny<int>())).ReturnsAsync(BuildVariation(existingItemGuid, updatedQuantity, newQuantity));

            var result = await _classUnderTest.Execute(15);

            result.Should().HaveCount(2);
            result.Should().ContainSingle(rsi => rsi.OldQuantity == originalQuantity && rsi.NewQuantity == updatedQuantity);
            result.Should().ContainSingle(rsi => rsi.OldQuantity == 0 && rsi.NewQuantity == newQuantity);
        }

        private static WorkOrder BuildWorkOrder(Guid itemId, int quantity)
        {
            return new WorkOrder
            {
                StatusCode = WorkStatusCode.VariationPendingApproval,
                WorkElements = new List<WorkElement>
               {
                   new WorkElement
                   {
                       RateScheduleItem = new List<RateScheduleItem>
                       {
                           new RateScheduleItem
                           {
                               CustomCode = "customCode",
                               CustomName = "customName",
                               Id = itemId,
                               Quantity = new Quantity(quantity)
                           }
                       }
                   }
               }
            };
        }

        private static JobStatusUpdate BuildVariation(Guid existingItemGuid, int updatedQuantity, int newQuantity)
        {
            return new JobStatusUpdate
            {
                MoreSpecificSORCode = new WorkElement
                {
                    RateScheduleItem = new List<RateScheduleItem>
                    {
                        new RateScheduleItem
                        {
                            Id = Guid.NewGuid(),
                            OriginalId = existingItemGuid,
                            CustomCode = "customCode",
                            CustomName = "customName",
                            Quantity = new Quantity(updatedQuantity)
                        },
                        new RateScheduleItem
                        {
                            Id = Guid.NewGuid(),
                            CustomCode = "newCode",
                            CustomName = "newName",
                            Quantity = new Quantity(newQuantity)
                        }
                    }
                }
            };
        }
    }
}
