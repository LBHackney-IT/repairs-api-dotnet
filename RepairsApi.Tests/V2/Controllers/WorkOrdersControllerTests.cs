using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Boundary;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RepairsApi.V2.Controllers.Parameters;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;
using RepairsApi.V2.Domain;
using RepairsApi.Tests.Helpers;
using Microsoft.FeatureManagement;
using RepairsApi.V2.UseCase;

namespace RepairsApi.Tests.V2.Controllers
{
    public class WorkOrdersControllerTests : ControllerTests
    {
        private WorkOrdersController _classUnderTest;
        private Mock<ICreateWorkOrderUseCase> _createWorkOrderUseCaseMock;
        private Mock<IListWorkOrdersUseCase> _listWorkOrdersUseCase;
        private Generator<WorkOrder> _generator;
        private Mock<ICompleteWorkOrderUseCase> _completeWorkOrderUseCase;
        private Mock<IUpdateJobStatusUseCase> _updateJobStatusUseCase;
        private Mock<IGetWorkOrderUseCase> _getWorkOrderUseCase;
        private Mock<IListWorkOrderTasksUseCase> _listWorkOrderTasksUseCase;
        private Mock<IListWorkOrderNotesUseCase> _listWorkOrderNotesUseCase;
        private Mock<IListVariationTasksUseCase> _listVariationsTaskUseCase;

        [SetUp]
        public void SetUp()
        {
            ConfigureGenerator();
            _createWorkOrderUseCaseMock = new Mock<ICreateWorkOrderUseCase>();
            _listWorkOrdersUseCase = new Mock<IListWorkOrdersUseCase>();
            _completeWorkOrderUseCase = new Mock<ICompleteWorkOrderUseCase>();
            _updateJobStatusUseCase = new Mock<IUpdateJobStatusUseCase>();
            _getWorkOrderUseCase = new Mock<IGetWorkOrderUseCase>();
            _listWorkOrderTasksUseCase = new Mock<IListWorkOrderTasksUseCase>();
            _listWorkOrderNotesUseCase = new Mock<IListWorkOrderNotesUseCase>();
            _listVariationsTaskUseCase = new Mock<IListVariationTasksUseCase>();
            _classUnderTest = new WorkOrdersController(
                _createWorkOrderUseCaseMock.Object,
                _listWorkOrdersUseCase.Object,
                _completeWorkOrderUseCase.Object,
                _updateJobStatusUseCase.Object,
                _getWorkOrderUseCase.Object,
                _listWorkOrderTasksUseCase.Object,
                _listWorkOrderNotesUseCase.Object,
                _listVariationsTaskUseCase.Object
            );
        }

        private void ConfigureGenerator()
        {
            _generator = new Generator<WorkOrder>()
                .AddWorkOrderGenerators();
        }

        [Test]
        public async Task ScheduleRepairReturnsOkWithObject()
        {
            // arrange
            const int newId = 2;
            _createWorkOrderUseCaseMock.Setup(m => m.Execute(It.IsAny<WorkOrder>()))
                .ReturnsAsync(new CreateOrderResult(newId, WorkStatusCode.Open, string.Empty));

            // act
            var result = await _classUnderTest.ScheduleRepair(new ScheduleRepair());

            // assert
            result.Should().BeOfType<OkObjectResult>();
            var createResult = GetResultData<CreateOrderResult>(result);
            createResult.Id.Should().Be(newId);
        }

        [Test]
        public async Task GetWorkOrders()
        {
            // arrange
            var expectedWorkOrders = CreateWorkOrders();

            // act
            var result = await _classUnderTest.GetList(new WorkOrderSearchParameters());

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<List<WorkOrderListItem>>()
                .Which.Should().HaveCount(expectedWorkOrders.Count);
        }

        [Test]
        public async Task ReturnsOkWhenCanCompleteWorkOrder()
        {
            // arrange
            const int expectedWorkOrderId = 4;
            var request = CreateRequest(expectedWorkOrderId);

            // act
            var response = await _classUnderTest.WorkOrderComplete(request);

            // assert
            response.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task ReturnsOkWhenCanUpdateJobStatus()
        {
            _updateJobStatusUseCase
                .Setup(uc => uc.Execute(It.IsAny<JobStatusUpdate>())).Returns(Task.CompletedTask);

            var response = await _classUnderTest.JobStatusUpdate(
                new JobStatusUpdate
                {
                    RelatedWorkOrderReference = new Reference
                    {
                        ID = "42"
                    }
                });

            response.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task ReturnsObjectFromUseCase()
        {
            var expectedWorkOrderResponse = new Generator<WorkOrderResponse>()
                .AddWorkOrderGenerators()
                .Ignore((WorkOrderResponse r) => r.ExternalAppointmentManagementUrl)
                .Generate();
            _getWorkOrderUseCase.Setup(uc => uc.Execute(It.IsAny<int>())).ReturnsAsync(expectedWorkOrderResponse);

            var result = await _classUnderTest.Get(1);

            GetStatusCode(result).Should().Be(200);
            GetResultData<WorkOrderResponse>(result).Should().Be(expectedWorkOrderResponse);
        }

        [Test]
        public async Task Returns404WhenCatching()
        {
            ResourceNotFoundException expectedException = new ResourceNotFoundException("test");
            _getWorkOrderUseCase.Setup(uc => uc.Execute(It.IsAny<int>())).ThrowsAsync(expectedException);

            var result = await _classUnderTest.Get(1);

            GetStatusCode(result).Should().Be(404);
            GetResultData<string>(result).Should().Be(expectedException.Message);
        }

        [Test]
        public async Task VariationListReturns()
        {
            var expected = new Generator<GetVariationResponse>().AddDefaultGenerators().Generate();
            _listVariationsTaskUseCase.Setup(uc => uc.Execute(1)).ReturnsAsync(expected);

            var result = await _classUnderTest.GetWorkOrderVariations(1);

            GetStatusCode(result).Should().Be(200);

            var response = GetResultData<GetVariationResponse>(result);

            response.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task TaskListReturns()
        {
            var expected = new Generator<WorkOrderTask>().AddDefaultGenerators().GenerateList(5);
            _listWorkOrderTasksUseCase.Setup(uc => uc.Execute(1)).ReturnsAsync(expected);

            var result = await _classUnderTest.ListWorkOrderTasks(1);

            GetStatusCode(result).Should().Be(200);

            var response = GetResultData<IEnumerable<WorkOrderItemViewModel>>(result);

            response.AssertForEach(expected, (vm, domain) =>
            {
                vm.Code.Should().Be(domain.Code);
                vm.Cost.Should().Be(domain.Cost);
                vm.DateAdded.Should().Be(domain.DateAdded);
                vm.Description.Should().Be(domain.Description);
                vm.Quantity.Should().Be(domain.Quantity);
                vm.Status.Should().Be(domain.Status);
            });
        }

        private List<WorkOrder> CreateWorkOrders()
        {
            var expectedWorkOrders = _generator.GenerateList(5);
            _listWorkOrdersUseCase.Setup(m => m.Execute(It.IsAny<WorkOrderSearchParameters>())).ReturnsAsync(expectedWorkOrders.Select(wo => wo.ToListItem()).ToList());
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

        [Test]
        public async Task NoteListReturns()
        {
            var expected = new Generator<NoteListItem>().AddDefaultGenerators().GenerateList(5);
            _listWorkOrderNotesUseCase.Setup(uc => uc.Execute(1)).ReturnsAsync(expected);

            var result = await _classUnderTest.ListWorkOrderNotes(1);

            GetStatusCode(result).Should().Be(200);

            var response = GetResultData<IEnumerable<NoteListItem>>(result);

            response.Should().BeEquivalentTo(expected);
        }
    }
}
