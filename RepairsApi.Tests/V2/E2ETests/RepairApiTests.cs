using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Infrastructure;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Quantity = RepairsApi.V2.Generated.Quantity;
using RateScheduleItem = RepairsApi.V2.Generated.RateScheduleItem;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;

namespace RepairsApi.Tests.V2.E2ETests
{
    public class RepairApiTests : MockWebApplicationFactory
    {

        [Test]
        public async Task ScheduleRepair()
        {
            // Arrange
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double> { 1 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .Generate();

            // Act
            var (code, response) = await Post<int>("/api/v2/repairs/schedule", request);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            var wo = GetWorkOrderFromDB(response);
            wo.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
        }

        [Test]
        public async Task ScheduleReturns401WhenLimitExceeded()
        {
            // Arrange
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double> { 1000 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .AddValue(TestDataSeeder.SorCode, (RateScheduleItem rsi) => rsi.CustomCode)
                .Generate();

            // Act
            var (code, response) = await Post<string>("/api/v2/repairs/schedule", request);

            // Assert
            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ViewElements()
        {
            var id = await CreateWorkOrder();

            var (code, response) = await Get<IEnumerable<WorkOrderItemViewModel>>($"/api/v2/repairs/{id}/tasks");

            code.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeEmpty();
            // TODO assert content
        }

        [Test]
        public async Task BadRequestWhenMultipleAmountsProvided()
        {
            // Arrange
            var request = GenerateWorkOrder<ScheduleRepair>().Generate();
            request.WorkElement.First().RateScheduleItem.First().Quantity.Amount.Add(3.5);

            // Act
            var code = await Post("/api/v2/repairs/schedule", request);

            // Assert
            code.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetListOfWorkOrders()
        {
            // Arrange
            var workOrderId = await CreateWorkOrder();

            // Act
            var (code, response) = await Get<List<WorkOrderListItem>>("/api/v2/repairs");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().Contain(wo => wo.Reference == workOrderId);
        }

        [Test]
        public async Task NewCompleteScheduleRepairWorkOrder()
        {
            // Arrange
            var id = await CreateWorkOrder();
            var request = new Generator<WorkOrderComplete>()
                            .AddWorkOrderCompleteGenerators()
                            .AddValue(id.ToString(), (WorkOrderComplete woc) => woc.WorkOrderReference.ID)
                            .AddValue(JobStatusUpdateTypeCode._0, (JobStatusUpdates jsu) => jsu.TypeCode)
                            .AddValue(CustomJobStatusUpdates.CANCELLED, (JobStatusUpdates jsu) => jsu.OtherType)
                            .Generate();

            // Act
            var code = await Post("/api/v2/workOrderComplete", request);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task CompleteRepairTwice()
        {
            // Arrange
            var woId = await CreateWorkOrder();
            await CompleteWorkOrder(woId);

            // Act
            var secondResponseCode = await CompleteWorkOrder(woId);

            // Assert
            secondResponseCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateSorCodes()
        {
            // Arrange
            string expectedCode = "expectedCode_UpdateWorkOrder";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);
            AddRateScheduleItem(workElement, expectedCode, 10);
            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);

            // Act
            var code = await Post("/api/v2/jobStatusUpdate", request);

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            // TODO Test a change has occured in the work order
        }

        [Test]
        public async Task CanViewNotes()
        {
            // Arrange
            var workOrderId = await CreateWorkOrder();
            await UpdateJob(workOrderId);

            // Act
            var (code, notes) = await Get<IList<NoteListItem>>($"/api/v2/repairs/{workOrderId}/notes");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            notes.Should().ContainSingle();
            // TODO Test note contents
        }

        [Test]
        public async Task UpdateReturns401WhenLimitExceeded()
        {
            // Arrange
            string expectedCode = "expectedCode_LimitExceededOnUpdate";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            AddRateScheduleItem(workElement, expectedCode, 100000);

            JobStatusUpdate request = CreateUpdateRequest(workOrderId, workElement);

            // Act
            var code = await Post("/api/v2/jobStatusUpdate", request);

            // Assert
            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetMissingWorkOrder()
        {
            // Act
            var code = await Get($"/api/v2/repairs/1000");

            // Assert
            code.Should().Be(404);
        }

        private static RepairsApi.V2.Generated.WorkElement TransformTasksToWorkElement(IEnumerable<WorkOrderItemViewModel> tasks)
        {
            return new RepairsApi.V2.Generated.WorkElement
            {
                RateScheduleItem = tasks.Select(task => new RateScheduleItem
                {
                    Id = task.Id,
                    CustomCode = task.Code,
                    CustomName = task.Description,
                    Quantity = new Quantity
                    {
                        Amount = new List<double>() { task.Quantity }
                    }
                }).ToList()
            };
        }

        private void AddTestCode(string expectedCode)
        {
            using var ctx = GetContext();
            TestDataSeeder.AddCode(ctx.DB, expectedCode);
        }

        private Generator<T> GenerateWorkOrder<T>()
        {
            Generator<T> gen = new Generator<T>();

            using (var ctx = GetContext())
            {
                var db = ctx.DB;
                gen = new Generator<T>()
                                .AddWorkOrderGenerators()
                                .AddValue(new List<double> { 0 }, (RateScheduleItem rsi) => rsi.Quantity.Amount);
            };

            return gen;
        }

        public WorkOrder GetWorkOrderFromDB(int id)
        {
            using var ctx = GetContext();
            var db = ctx.DB;
            var repair = db.WorkOrders.Find(id);
            return repair;
        }

        public async Task<HttpStatusCode> CompleteWorkOrder(int id)
        {
            var request = new Generator<WorkOrderComplete>()
                .AddWorkOrderCompleteGenerators()
                .AddValue(id.ToString(), (WorkOrderComplete woc) => woc.WorkOrderReference.ID)
                .AddValue(JobStatusUpdateTypeCode._0, (JobStatusUpdates jsu) => jsu.TypeCode)
                .AddValue(CustomJobStatusUpdates.CANCELLED, (JobStatusUpdates jsu) => jsu.OtherType)
                .Generate();

            return await Post("/api/v2/workOrderComplete", request);
        }

        private async Task UpdateJob(int workOrderId)
        {
            var tasks = await GetTasks(workOrderId);
            RepairsApi.V2.Generated.WorkElement workElement = TransformTasksToWorkElement(tasks);

            JobStatusUpdate request = new Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue(workElement, (JobStatusUpdate jsu) => jsu.MoreSpecificSORCode)
                .AddValue("comments", (JobStatusUpdate jsu) => jsu.Comments)
                .Generate();

            await Post("/api/v2/jobStatusUpdate", request);
        }

        private async Task<int> CreateWorkOrder()
        {
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double> { 1 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .Generate();

            var (_, response) = await Post<int>("/api/v2/repairs/schedule", request);

            return response;
        }

        public async Task<IEnumerable<WorkOrderItemViewModel>> GetTasks(int workOrderId)
        {
            var (_, response) = await Get<IEnumerable<WorkOrderItemViewModel>>($"/api/v2/repairs/{workOrderId}/tasks");

            return response;
        }

        private static JobStatusUpdate CreateUpdateRequest(int workOrderId, RepairsApi.V2.Generated.WorkElement workElement)
        {
            return new Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue(workElement, (JobStatusUpdate jsu) => jsu.MoreSpecificSORCode)
                .AddValue("comments", (JobStatusUpdate jsu) => jsu.Comments)
                .Generate();
        }

        private static void AddRateScheduleItem(RepairsApi.V2.Generated.WorkElement workElement, string code, int quantity)
        {
            workElement.RateScheduleItem.Add(new RateScheduleItem
            {
                CustomCode = code,
                CustomName = "test code",
                Quantity = new Quantity
                {
                    Amount = new double[] { quantity }
                }
            });
        }
    }
}
