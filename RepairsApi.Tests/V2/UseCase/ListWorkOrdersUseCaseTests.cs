using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class ListWorkOrdersUseCaseTests
    {
        private ListWorkOrdersUseCase _classUnderTest;
        private MockRepairsGateway _repairsMock;
        private Mock<IScheduleOfRatesGateway> _sorGatewayMock;
        private Generator<WorkOrder> _generator;

        [SetUp]
        public void Setup()
        {
            configureGenerator();
            _repairsMock = new MockRepairsGateway();
            _sorGatewayMock = new Mock<IScheduleOfRatesGateway>();
            _classUnderTest = new ListWorkOrdersUseCase(_repairsMock.Object, _sorGatewayMock.Object);
        }

        private void configureGenerator()
        {
            _generator = new Generator<WorkOrder>()
                .AddDefaultValueGenerators();
        }

        [Test]
        public async Task ReturnsWorkOrders()
        {
            //Arrange
            const int expectedWorkOrderCount = 5;
            GenerateAndReturnWorkOrders(expectedWorkOrderCount);

            //Act
            var workOrders = await _classUnderTest.Execute(new WorkOrderSearchParameters());

            //Assert
            workOrders.Should().HaveCount(expectedWorkOrderCount);
        }

        [Test]
        public async Task CanFilterByContractorRef()
        {
            // Arrange
            var expectedCode = AddSorCode(Guid.NewGuid().ToString());

            var allWorkOrders = GenerateWorkOrders(5, "not" + expectedCode.CustomCode);
            var expectedWorkOrders = GenerateWorkOrders(3, expectedCode.CustomCode);
            allWorkOrders.AddRange(expectedWorkOrders);

            _repairsMock.ReturnsWorkOrders(allWorkOrders);

            var workOrderSearchParameters = new WorkOrderSearchParameters
            {
                ContractorReference = expectedCode.SORContractorRef
            };

            // Act
            var workOrders = await _classUnderTest.Execute(workOrderSearchParameters);

            // Assert
            var expectedResponses = expectedWorkOrders.Select(ewo => ewo.ToResponse());
            workOrders.Should().BeEquivalentTo(expectedResponses);
        }

        [Test]
        public async Task ReturnsCorrectPageOfWorkOrders()
        {
            //Arrange
            const int workOrderCount = 50;
            const int expectedPageSize = 10;
            var generatedWorkOrders = GenerateAndReturnWorkOrders(workOrderCount);
            var workOrderSearchParameters = new WorkOrderSearchParameters
            {
                PageNumber = 2,
                PageSize = expectedPageSize
            };

            //Act
            var workOrders = await _classUnderTest.Execute(workOrderSearchParameters);

            //Assert
            var expectedResult = generatedWorkOrders.OrderByDescending(wo => wo.DateRaised)
                .Skip((workOrderSearchParameters.PageNumber - 1) * workOrderSearchParameters.PageSize)
                .Take(workOrderSearchParameters.PageSize)
                .Select(wo => wo.ToResponse());
            workOrders.Should().BeEquivalentTo(expectedResult);
        }

        private List<WorkOrder> GenerateWorkOrders(int number, string expectedCode)
        {

            var otherWorkOrders = _generator.GenerateList(number);
            SetSorCodes(expectedCode, otherWorkOrders.ToArray());
            return otherWorkOrders;
        }

        private List<WorkOrder> GenerateAndReturnWorkOrders(int workOrderCount)
        {

            var generatedWorkOrders = _generator.GenerateList(workOrderCount);
            _repairsMock.Setup(r => r.GetWorkOrders())
                .ReturnsAsync(generatedWorkOrders);
            return generatedWorkOrders;
        }

        private static void SetSorCodes(string expectedCode, params WorkOrder[] expectedWorkOrders)
        {
            foreach (var workOrder in expectedWorkOrders)
            {
                foreach (var workElement in workOrder.WorkElements)
                {
                    foreach (var rateScheduleItem in workElement.RateScheduleItem)
                    {
                        rateScheduleItem.CustomCode = expectedCode;
                    }
                }
            }
        }

        private ScheduleOfRates AddSorCode(string contractorRef = "contractor")
        {
            var expectedCode = new ScheduleOfRates
            {
                CustomCode = "1",
                CustomName = "name",
                SORContractorRef = contractorRef,
                Priority = new SORPriority
                {
                    Description = "priorityDescription",
                    PriorityCode = 1
                }
            };
            var expectedCodes = new List<ScheduleOfRates>
            {
                expectedCode
            };
            _sorGatewayMock.Setup(g => g.GetSorCodes(It.IsAny<string>()))
                .ReturnsAsync(expectedCodes);
            return expectedCode;
        }
    }

    public class MockRepairsGateway : Mock<IRepairsGateway>
    {
        private IEnumerable<WorkOrder> _workOrders;

        public void ReturnsWorkOrders(List<WorkOrder> workOrders)
        {
            _workOrders = workOrders;

            Setup(g => g.GetWorkOrders(It.IsAny<Expression<Func<WorkOrder, bool>>[]>()))
                .ReturnsAsync((Expression<Func<WorkOrder, bool>>[] expressions) =>
                {
                    var tempWorkOrders = _workOrders;
                    foreach (var whereExpression in expressions)
                    {
                        tempWorkOrders = tempWorkOrders.Where(whereExpression.Compile());
                    }
                    return tempWorkOrders;
                });
        }
    }
}
