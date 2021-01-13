using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Obj.Genarator;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.Infrastructure;
using RepairsApi.V1.UseCase;

namespace RepairsApi.Tests.V1.UseCase
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
            _generator = new Generator<WorkOrder>(new Dictionary<Type, IGenerator>
            {
                {
                    typeof(string), new RandomStringGenerator(10)
                },
                {
                    typeof(double), new RandomDoubleGenerator(0, 50)
                },
                {
                    typeof(bool), new RandomBoolGenerator()
                }
            });
        }

        [Test]
        public void ReturnsWorkOrders()
        {
            //Arrange
            const int expectedWorkOrderCount = 5;
            _repairsMock.Setup(r => r.GetWorkOrders())
                .Returns(_generator.GenerateList(expectedWorkOrderCount));

            //Act
            var workOrders = _classUnderTest.Execute();

            //Assert
            workOrders.Should().HaveCount(expectedWorkOrderCount);
        }
    }
}
