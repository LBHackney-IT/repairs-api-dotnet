using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;

namespace RepairsApi.Tests.V2.Controllers
{
    public class RepairsControllerTests : ControllerTests
    {
        private RepairsController _classUnderTest;
        private Mock<IRaiseRepairUseCase> _raiseRepairUseCaseMock;
        private Mock<IListWorkOrdersUseCase> _listWorkOrdersUseCase;
        private Generator<WorkOrder> _generator;
        private Mock<ICompleteWorkOrderUseCase> _completeWorkOrderUseCase;

        [SetUp]
        public void SetUp()
        {
            ConfigureGenerator();
            _raiseRepairUseCaseMock = new Mock<IRaiseRepairUseCase>();
            _listWorkOrdersUseCase = new Mock<IListWorkOrdersUseCase>();
            _completeWorkOrderUseCase = new Mock<ICompleteWorkOrderUseCase>();
            _classUnderTest = new RepairsController(
                _raiseRepairUseCaseMock.Object,
                _listWorkOrdersUseCase.Object,
                _completeWorkOrderUseCase.Object
                );
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
            var expectedWorkOrders = CreateWorkOrders();

            // act
            var result = _classUnderTest.GetList();

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<List<WorkOrderListItem>>()
                .Which.Should().HaveCount(expectedWorkOrders.Count);
        }

        [Test]
        public async Task ReturnsOkWhenCanCompleteWorkOrder()
        {
            // arrange
            UseCaseReturns(true);
            const int expectedWorkOrderId = 4;
            var request = CreateRequest(expectedWorkOrderId);

            // act
            var response = await _classUnderTest.WorkOrderComplete(request);

            // assert
            response.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task ReturnsBadRequestWhenCantCompleteWorkOrder()
        {
            // arrange
            UseCaseReturns(false);
            const int expectedWorkOrderId = 4;
            var request = CreateRequest(expectedWorkOrderId);

            // act
            var response = await _classUnderTest.WorkOrderComplete(request);

            // assert
            response.Should().BeOfType<BadRequestResult>();
        }

        private void UseCaseReturns(bool result)
        {
            _completeWorkOrderUseCase.Setup(uc => uc.Execute(It.IsAny<WorkOrderComplete>()))
                .ReturnsAsync(result);
        }

        private List<WorkOrder> CreateWorkOrders()
        {

            var expectedWorkOrders = _generator.GenerateList(5);
            _listWorkOrdersUseCase.Setup(m => m.Execute()).Returns(expectedWorkOrders.Select(wo => wo.ToResponse()).ToList());
            return expectedWorkOrders;
        }

        private static WorkOrderComplete CreateRequest(int expectedWorkOrderId)
        {
            var request = new WorkOrderComplete
            {
                WorkOrderReference = new Reference
                {
                    ID = expectedWorkOrderId.ToString()
                }
            };
            return request;
        }
    }
}