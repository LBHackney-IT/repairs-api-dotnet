using System;
using System.Collections.Generic;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.V2.Gateways
{
    public class RepairGatewayTests
    {
        private RepairsGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new RepairsGateway(InMemoryDb.Instance);
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CanCreateWorkOrder()
        {
            // arrange
            var expected = CreateWorkOrder();

            // act
            await _classUnderTest.CreateWorkOrder(expected);
            await InMemoryDb.Instance.SaveChangesAsync();

            // assert
            InMemoryDb.Instance.WorkOrders.Should().ContainSingle().Which.IsSameOrEqualTo(expected);
        }

        [Test]
        public async Task CanGetWorkOrders()
        {
            // arrange
            var expectedWorkOrders = CreateWorkOrder();
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrders);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = _classUnderTest.GetWorkOrders();

            // assert
            workOrders.Should().ContainSingle().Which.Should().BeEquivalentTo(expectedWorkOrders);
        }

        private static WorkOrder CreateWorkOrder()
        {

            var expected = new WorkOrder
            {
                WorkPriority = new WorkPriority
                {
                    PriorityCode = RepairsApi.V2.Generated.WorkPriorityCode._1,
                    RequiredCompletionDateTime = DateTime.UtcNow
                },
                WorkClass = new WorkClass
                {
                    WorkClassCode = RepairsApi.V2.Generated.WorkClassCode._0
                },
                WorkElements = new List<WorkElement>(),
                DescriptionOfWork = "description"
            };
            return expected;
        }
    }
}
