using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.UseCase
{
    [TestFixture]
    public class ListWorkOrdersUseCaseTests
    {
        private ListWorkOrdersUseCase _classUnderTest;
        private Mock<IRepairsGateway> _repairsMock;
        private Generator<WorkOrder> _generator;

        [SetUp]
        public void Setup()
        {
            configureGenerator();
            _repairsMock = new Mock<IRepairsGateway>();
            _classUnderTest = new ListWorkOrdersUseCase(_repairsMock.Object);
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
            _repairsMock.Setup(r => r.GetWorkOrders())
                .ReturnsAsync(_generator.GenerateList(expectedWorkOrderCount));

            //Act
            var workOrders = await _classUnderTest.Execute(new WorkOrderSearchParamters());

            //Assert
            workOrders.Should().HaveCount(expectedWorkOrderCount);
        }
    }
}
