using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    public class UpdateSorCodeUseCaseTests
    {
        private UpdateSorCodesUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new UpdateSorCodesUseCase();
        }

        [Test]
        public async Task MoreSpecificSORCodeAddsSORCodeToWorkOrder()
        {
            Guid originalGuid = Guid.NewGuid();
            Guid newGuid = Guid.NewGuid();
            const int originalQuantity = 10;
            const int newQuantity = 15;

            var workOrder = BuildWorkOrder(originalGuid, originalQuantity);

            var workElement = CreateWorkElement(workOrder);

            workElement.RateScheduleItem.Add(new RateScheduleItem
            {
                Id = newGuid,
                Quantity = new Quantity(newQuantity)
            });

            await _classUnderTest.Execute(workOrder, workElement);

            workOrder.WorkElements.Single().RateScheduleItem.Should().HaveCount(2);
            workOrder.WorkElements.Single().RateScheduleItem.Should().ContainSingle(rsi => rsi.Quantity.Amount == originalQuantity);
            workOrder.WorkElements.Single().RateScheduleItem.Should().ContainSingle(rsi => rsi.Quantity.Amount == newQuantity);
        }

        [Test]
        public async Task UpdateQuantityOfExistingCodes()
        {
            const int newQuantity = 15;

            WorkOrder workOrder = BuildWorkOrder(Guid.NewGuid(), 10);

            WorkElement workElement = CreateWorkElement(workOrder);
            workElement.RateScheduleItem.First().Quantity.Amount = newQuantity;

            await _classUnderTest.Execute(workOrder, workElement);

            workOrder.WorkElements.Single().RateScheduleItem.Should().HaveCount(1);
            workOrder.WorkElements.Single().RateScheduleItem.Should().ContainSingle(rsi => rsi.Quantity.Amount == newQuantity);
        }

        private static WorkElement CreateWorkElement(WorkOrder workOrder)
        {
            var workElement = workOrder.WorkElements.First().DeepClone();
            workElement.RateScheduleItem.ForEach(r => r.OriginalId = r.Id);
            return workElement;
        }

        [Test]
        public async Task ThrowUnsupportedWhenOriginalSorCodeNotPresent()
        {
            Guid originalGuid = Guid.NewGuid();
            Guid newGuid = Guid.NewGuid();
            var workOrder = BuildWorkOrder(originalGuid, 10);

            var workElement = new WorkElement()
            {
                RateScheduleItem = new List<RateScheduleItem>()
            };

            Func<Task> testFn = async () => await _classUnderTest.Execute(workOrder, workElement);

            await testFn.Should().ThrowAsync<NotSupportedException>();
        }

        private static WorkOrder BuildWorkOrder(Guid originalGuid, int originalQuantity)
        {
            return new WorkOrder
            {
                WorkElements = new List<WorkElement>
                {
                    new WorkElement
                    {
                        RateScheduleItem = new List<RateScheduleItem>
                        {
                            new RateScheduleItem
                            {
                                Id = originalGuid,
                                Quantity = new Quantity(originalQuantity)
                            }
                        }
                    }
                }
            };
        }
    }
}
