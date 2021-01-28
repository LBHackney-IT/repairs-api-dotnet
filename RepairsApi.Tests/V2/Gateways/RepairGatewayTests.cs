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
            var workOrders = await _classUnderTest.GetWorkOrders();

            // assert
            workOrders.Should().ContainSingle().Which.Should().BeEquivalentTo(expectedWorkOrders);
        }

        [Test]
        public async Task CanGetWorkOrdersFiltered()
        {
            // arrange
            await InMemoryDb.Instance.WorkOrders.AddRangeAsync(CreateWorkOrders(25));
            var expectedWorkOrder = CreateWorkOrder();
            expectedWorkOrder.ParkingArrangements = Guid.NewGuid().ToString();
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = await _classUnderTest.GetWorkOrders(wo =>
                wo.ParkingArrangements == expectedWorkOrder.ParkingArrangements);

            // assert
            workOrders.Should().ContainSingle().Which.Should().BeEquivalentTo(expectedWorkOrder);
        }

        [Test]
        public async Task CanGetWorkOrderById()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = await _classUnderTest.GetWorkOrder(expectedWorkOrder.Id);

            // assert
            workOrders.Should().BeEquivalentTo(expectedWorkOrder);
        }

        [Test]
        public async Task CanGetWorkElements()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            expectedWorkOrder.WorkElements.Add(new WorkElement{Id = Guid.NewGuid()});
            expectedWorkOrder.WorkElements.Add(new WorkElement{Id = Guid.NewGuid()});

            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workElements = await _classUnderTest.GetWorkElementsForWorkOrder(expectedWorkOrder);

            // assert
            workElements.Should().BeEquivalentTo(expectedWorkOrder.WorkElements);
        }

        private static ICollection<WorkOrder> CreateWorkOrders(int count)
        {
            var list = new List<WorkOrder>();

            for (var i = 0; i < count; i++)
            {
                list.Add(CreateWorkOrder());
            }

            return list;
        }

        private static WorkOrder CreateWorkOrder()
        {

            var expected = new WorkOrder
            {
                WorkPriority = new WorkPriority
                {
                    PriorityCode = RepairsApi.V2.Generated.WorkPriorityCode._1, RequiredCompletionDateTime = DateTime.UtcNow
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
