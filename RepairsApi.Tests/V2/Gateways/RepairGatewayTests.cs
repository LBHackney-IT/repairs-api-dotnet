using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RepairsApi.V2.Gateways;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using Moq;
using RepairsApi.Tests.Helpers;

namespace RepairsApi.Tests.V2.Gateways
{
    public class RepairGatewayTests
    {
        private CurrentUserServiceMock _userServiceMock;
        private RepairsGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new CurrentUserServiceMock();
            _classUnderTest = new RepairsGateway(InMemoryDb.Instance, _userServiceMock.Object);
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
            var expectedWorkOrders = CreateWorkOrder("contractor");
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrders);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = await _classUnderTest.GetWorkOrders();

            // assert
            workOrders.Should().ContainSingle().Which.Should().BeEquivalentTo(expectedWorkOrders);
        }

        [TestCase("contractor", "othercontractor")]
        [TestCase("contractor", "othercontractor", "otherContractor2")]
        public async Task FilterBasedOnContractors(string woContractor, params string[] userContractors)
        {
            // arrange
            var expectedWorkOrders = CreateWorkOrder(woContractor);
            _userServiceMock.SetContractor(userContractors);
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrders);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = await _classUnderTest.GetWorkOrders();

            // assert
            workOrders.Should().BeEmpty();
        }

        [TestCase("contractor", "contractor")]
        [TestCase("contractor", "contractor", "otherContractor")]
        public async Task GetWorkOrdersForValidContractors(string woContractor, params string[] userContractors)
        {
            // arrange
            var expectedWorkOrders = CreateWorkOrder(woContractor);
            _userServiceMock.SetContractor(userContractors);
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrders);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = await _classUnderTest.GetWorkOrders();

            // assert
            workOrders.Should().Contain(expectedWorkOrders);
        }

        [Test]
        public async Task CanGetWorkOrdersFiltered()
        {
            // arrange
            await InMemoryDb.Instance.WorkOrders.AddRangeAsync(CreateWorkOrders(25));
            var expectedWorkOrder = CreateWorkOrder("contractor");
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
        public async Task CanGetWorkOrdersFilteredByStatus()
        {
            // arrange
            await InMemoryDb.Instance.WorkOrders.AddRangeAsync(CreateWorkOrders(25));
            var expectedWorkOrder = CreateWorkOrderWithStatus(70);
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = await _classUnderTest.GetWorkOrders(wo =>
                wo.StatusCode == WorkStatusCode.Hold);

            // assert
            workOrders.Should().ContainSingle().Which.Should().BeEquivalentTo(expectedWorkOrder);
        }

        [TestCase("contractor", "contractor")]
        [TestCase("contractor", "contractor", "otherContractor")]
        public async Task CanGetWorkOrderById(string woContractor, params string[] userContractors)
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder(woContractor);
            _userServiceMock.SetContractor(userContractors);
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workOrders = await _classUnderTest.GetWorkOrder(expectedWorkOrder.Id);

            // assert
            workOrders.Should().BeEquivalentTo(expectedWorkOrder);
        }


        [TestCase("contractor", "othercontractor")]
        [TestCase("contractor", "othercontractor", "otherContractor2")]
        public async Task ThrowsGettingOtherContractorsWorkOrder(string woContractor, params string[] userContractors)
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder(woContractor);
            _userServiceMock.SetContractor(userContractors);
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            Func<Task> fn = async () => await _classUnderTest.GetWorkOrder(expectedWorkOrder.Id);

            // assert
            await fn.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task CanUpdateWorkStatus()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            await _classUnderTest.UpdateWorkOrderStatus(expectedWorkOrder.Id, WorkStatusCode.Complete);
            var workOrder = await _classUnderTest.GetWorkOrder(expectedWorkOrder.Id);

            // assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.Complete);
        }

        [Test]
        public async Task CanGetWorkElements()
        {
            // arrange
            var expectedWorkOrder = CreateWorkOrder();
            expectedWorkOrder.WorkElements.Add(new WorkElement { Id = Guid.NewGuid() });
            expectedWorkOrder.WorkElements.Add(new WorkElement { Id = Guid.NewGuid() });

            await InMemoryDb.Instance.WorkOrders.AddAsync(expectedWorkOrder);
            await InMemoryDb.Instance.SaveChangesAsync();

            // act
            var workElements = await _classUnderTest.GetWorkElementsForWorkOrder(expectedWorkOrder);

            // assert
            workElements.Should().BeEquivalentTo(expectedWorkOrder.WorkElements);
        }

        private ICollection<WorkOrder> CreateWorkOrders(int count)
        {
            var list = new List<WorkOrder>();

            for (var i = 0; i < count; i++)
            {
                list.Add(CreateWorkOrder());
            }

            return list;
        }

        private WorkOrder CreateWorkOrder(string contractor = null)
        {
            _userServiceMock.SetContractor(contractor);

            var expected = new WorkOrder
            {
                WorkPriority = new WorkPriority
                {
                    PriorityCode = 1,
                    RequiredCompletionDateTime = DateTime.UtcNow
                },
                WorkClass = new WorkClass
                {
                    WorkClassCode = RepairsApi.V2.Generated.WorkClassCode._0
                },
                WorkElements = new List<WorkElement>(),
                DescriptionOfWork = "description",
                AssignedToPrimary = new Party
                {
                    ContractorReference = contractor
                }
            };
            return expected;
        }

        private static WorkOrder CreateWorkOrderWithStatus(int statusCode = 0)
        {
            var expected = new WorkOrder
            {
                WorkPriority = new WorkPriority
                {
                    PriorityCode = 1,
                    RequiredCompletionDateTime = DateTime.UtcNow
                },
                WorkClass = new WorkClass
                {
                    WorkClassCode = RepairsApi.V2.Generated.WorkClassCode._0
                },
                WorkElements = new List<WorkElement>(),
                DescriptionOfWork = "description",
                StatusCode = (WorkStatusCode) Enum.Parse(typeof(WorkStatusCode), statusCode.ToString())
            };
            return expected;
        }
    }
}
