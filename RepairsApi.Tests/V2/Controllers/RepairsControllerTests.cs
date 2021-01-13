using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.Tests.V2.Controllers
{
    public class RepairsControllerTests : ControllerTests
    {
        private RepairsController _classUnderTest;
        private Mock<IRaiseRepairUseCase> _raiseRepairUseCaseMock;
        private Mock<IListWorkOrdersUseCase> _listWorkOrdersUseCase;
        private Generator<WorkOrder> _generator;

        [SetUp]
        public void SetUp()
        {
            ConfigureGenerator();
            _raiseRepairUseCaseMock = new Mock<IRaiseRepairUseCase>();
            _listWorkOrdersUseCase = new Mock<IListWorkOrdersUseCase>();
            _classUnderTest = new RepairsController(_raiseRepairUseCaseMock.Object, _listWorkOrdersUseCase.Object);
        }

        private void ConfigureGenerator()
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
        public async Task CreateReturnsOkWithInt()
        {
            // arrange
            const int newId = 2;
            _raiseRepairUseCaseMock.Setup(m => m.Execute(It.IsAny<WorkOrder>())).ReturnsAsync(newId);

            // act
            var result = await _classUnderTest.RaiseRepair(new RaiseRepair());

            // assert
            result.Should().BeOfType<OkObjectResult>();
            GetResultData<int>(result).Should().Be(newId);
        }

        [Test]
        public void GetWorkOrders()
        {
            // arrange
            var expectedWorkOrders = _generator.GenerateList(5).Select(wo => wo.ToResponse()).ToList();
            _listWorkOrdersUseCase.Setup(m => m.Execute()).Returns(expectedWorkOrders);

            // act
            var result = _classUnderTest.GetList();

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<List<WorkOrderListItem>>()
                .Which.Should().HaveCount(expectedWorkOrders.Count);
        }
    }
}
