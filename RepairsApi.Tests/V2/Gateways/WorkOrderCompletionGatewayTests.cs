using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    public class WorkOrderCompletionGatewayTests
    {
        private WorkOrderCompletionGateway _classUnderTest;
        private Generator<WorkOrderComplete> _generator;

        [SetUp]
        public void Setup()
        {
            SetupGenerator();
            _classUnderTest = new WorkOrderCompletionGateway(InMemoryDb.Instance);
        }

        private void SetupGenerator()
        {
            _generator = new Generator<WorkOrderComplete>()
                .AddInfrastructureWorkOrderGenerators();
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CanCreateWorkOrderCompletion()
        {
            // arrange
            var expected = CreateWorkOrderCompletion();

            // act
            await _classUnderTest.CreateWorkOrderCompletion(expected);
            await InMemoryDb.Instance.SaveChangesAsync();

            // assert
            InMemoryDb.Instance.WorkOrderCompletes.Should().ContainSingle().Which.IsSameOrEqualTo(expected);
        }

        [Test]
        public async Task IsCompleteReturnsTrueForCompletedWorkOrder()
        {
            var workOrderId = await AddWorkOrderCompletion();

            var isComplete = await _classUnderTest.IsWorkOrderCompleted(workOrderId);

            isComplete.Should().BeTrue();
        }


        [Test]
        public async Task IsCompleteReturnsFalseForNotCompletedWorkOrder()
        {
            var expectedWorkOrder = CreateWorkOrderCompletion().WorkOrder;

            var entry = await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            var isComplete = await _classUnderTest.IsWorkOrderCompleted(entry.Entity.Id);

            isComplete.Should().BeFalse();
        }


        private async Task<int> AddWorkOrderCompletion()
        {
            var completion = CreateWorkOrderCompletion();

            var entry = await InMemoryDb.Instance.WorkOrderCompletes.AddAsync(completion);
            await InMemoryDb.Instance.SaveChangesAsync();

            return entry.Entity.WorkOrder.Id;
        }

        private WorkOrderComplete CreateWorkOrderCompletion()
        {
            return _generator.Generate();
        }
    }
}
