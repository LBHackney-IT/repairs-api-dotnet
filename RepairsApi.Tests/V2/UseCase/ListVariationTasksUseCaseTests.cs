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
        public async Task GetsTasks()
        {
            const int originalQuantity = 25;
            const int currentQuantity = 10;
            const int updatedQuantity = 15;
            const int newQuantity = 5;
            const string notes = "notes";
            const string authorName = "expectedAuthor";
            Guid existingItemGuid = Guid.NewGuid();
            _repairsGatewayMock.Setup(g => g.GetWorkOrder(It.IsAny<int>())).ReturnsAsync(BuildWorkOrder(existingItemGuid, currentQuantity, originalQuantity));
            _jobStatusUpdateGatewayMock.Setup(g => g.GetOutstandingVariation(It.IsAny<int>())).ReturnsAsync(BuildVariation(existingItemGuid, updatedQuantity, newQuantity, notes, authorName));

            var result = await _classUnderTest.Execute(15);

            result.Tasks.Should().HaveCount(2);
            result.Tasks.Should().ContainSingle(rsi => rsi.CurrentQuantity == currentQuantity && rsi.VariedQuantity == updatedQuantity && rsi.OriginalQuantity == originalQuantity);
            result.Tasks.Should().ContainSingle(rsi => rsi.CurrentQuantity == 0 && rsi.VariedQuantity == newQuantity && rsi.AuthorName == authorName);
            result.Notes.Should().Be(notes);
        }

        private static WorkOrder BuildWorkOrder(Guid itemId, int currentQuantity, int originalQuantity)
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
                               Quantity = new Quantity(currentQuantity),
                               OriginalQuantity = originalQuantity
                           }
                       }
                   }
               }
            };
        }

        private static JobStatusUpdate BuildVariation(Guid existingItemGuid, int updatedQuantity, int newQuantity, string notes, string authorName)
        {
            return new JobStatusUpdate
            {
                Comments = notes,
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
                },
                AuthorName = authorName
            };
        }
    }
}
