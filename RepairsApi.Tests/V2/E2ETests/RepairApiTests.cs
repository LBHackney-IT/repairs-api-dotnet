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
using RepairsApi.V2.Factories;

namespace RepairsApi.Tests.V2.E2ETests
{

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Tests")]
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

        private async Task<int> CreateWorkOrder()
        {
            var request = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double> { 1 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .Generate();

            var (_, response) = await Post<int>("/api/v2/repairs/schedule", request);

            return response;
        }

        [Test]
        public async Task ViewElements()
        {
            var id = await CreateWorkOrder();

            var (code, response) = await Get<IEnumerable<WorkOrderItemViewModel>>($"/api/v2/repairs/{id}/tasks");

            code.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeEmpty(); // More precise check
        }

        public async Task<IEnumerable<WorkOrderItemViewModel>> GetTasks(int workOrderId)
        {
            var (_, response) = await Get<IEnumerable<WorkOrderItemViewModel>>($"/api/v2/repairs/{workOrderId}/tasks");

            return response;
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
        public async Task UpdateScheduleRepairWorkOrder()
        {
            string expectedCode = "expectedCodeUpdateWorkOrder";
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();
            var tasks = await GetTasks(workOrderId);

            var workElement = request.WorkElement.First();
            var expectedNewCode = new RateScheduleItem
            {
                CustomCode = expectedCode,
                Quantity = new Quantity
                {
                    Amount = new List<double>
                    {
                        quantity
                    }
                }
            };
            workElement.RateScheduleItem.Add(expectedNewCode);

            Generator<JobStatusUpdate> generator = new Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue(workElement, (JobStatusUpdate jsu) => jsu.MoreSpecificSORCode)
                .AddValue("comments", (JobStatusUpdate jsu) => jsu.Comments);

            var updateRequest = generator.Generate();

        }

        [Test]
        public async Task UpdateReturns401WhenLimitExceeded()
        {
            var expectedCode = TestDataSeeder.SorCode;
            var (_, response) = await ScheduleAndUpdateWorkOrder(expectedCode, "comments", 1000);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task CanViewNotes()
        {
            const string expectedNote = "expectedNote";
            const string expectedCode = "expectedCodeCanViewNotes";

            AddTestCode(expectedCode);
            var (workOrderId, httpResponseMessage) = await ScheduleAndUpdateWorkOrder(expectedCode, expectedNote);
            await ValidateUpdate(httpResponseMessage, workOrderId, expectedCode);

            var client = CreateClient();
            var response = await client.GetAsync(new Uri($"/api/v2/repairs/{workOrderId}/notes", UriKind.Relative));

            response.StatusCode.Should().Be(HttpStatusCode.OK, response.Content.ToString());
            string responseContent = await response.Content.ReadAsStringAsync();
            var notes = JsonConvert.DeserializeObject<IList<NoteListItem>>(responseContent);
            notes.Should().ContainSingle(n => n.Note == expectedNote);
        }

        private void AddTestCode(string expectedCode)
        {
            using var ctx = GetContext();
            TestDataSeeder.AddCode(ctx.DB, expectedCode);
        }

        [Test]
        public async Task GetMissingWorkOrder()
        {
            var client = CreateClient();

            var response = await client.GetAsync(new Uri($"/api/v2/repairs/1000", UriKind.Relative));

            response.StatusCode.Should().Be(404);
        }

        private async Task<(int workOrderId, HttpResponseMessage response)> ScheduleAndUpdateWorkOrder(string expectedCode, string updateComments = "comments", int quantity = 0)
        {

            string endpoint = "/api/v2/repairs/schedule";
            Generator<ScheduleRepair> requestGenerator = GenerateWorkOrder<ScheduleRepair>();

            var request = requestGenerator.Generate();

            var client = CreateClient();

            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var workElement = request.WorkElement.First();
            var workOrderId = await ValidateWorkOrderCreation(client, content, wo =>
            {
                workElement.RateScheduleItem = wo.WorkElements.First().RateScheduleItem.Select(rsi => rsi.ToResponse()).ToList();
            }, endpoint);

            var expectedNewCode = new RateScheduleItem
            {
                CustomCode = expectedCode,
                Quantity = new Quantity
                {
                    Amount = new List<double>
                    {
                        quantity
                    }
                }
            };
            workElement.RateScheduleItem.Add(expectedNewCode);

            Generator<JobStatusUpdate> generator = new Generator<JobStatusUpdate>()
                .AddJobStatusUpdateGenerators()
                .AddValue(JobStatusUpdateTypeCode._80, (JobStatusUpdate jsu) => jsu.TypeCode)
                .AddValue(workOrderId.ToString(), (JobStatusUpdate jsu) => jsu.RelatedWorkOrderReference.ID)
                .AddValue(workElement, (JobStatusUpdate jsu) => jsu.MoreSpecificSORCode)
                .AddValue(updateComments, (JobStatusUpdate jsu) => jsu.Comments);

            var updateRequest = generator.Generate();

            client.SetGroup(GetGroup(TestDataSeeder.Contractor));
            var serializedUpdateContent = JsonConvert.SerializeObject(updateRequest);
            StringContent updateContent = new StringContent(serializedUpdateContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(new Uri("/api/v2/jobStatusUpdate", UriKind.Relative), updateContent);

            return (workOrderId, response);
        }

        private async Task ValidateUpdate(HttpResponseMessage updateResponse, int workOrderId, string expectedNewCode)
        {
            var client = CreateClient();
            client.SetGroup(GetGroup(TestDataSeeder.Contractor));

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK, await updateResponse.Content.ReadAsStringAsync());

            // get the work order to validate new code is on there
            var response = await client.GetAsync(new Uri($"/api/v2/repairs/{workOrderId}/tasks", UriKind.Relative));
            string responseContent = await response.Content.ReadAsStringAsync();
            var workOrderItems = JsonConvert.DeserializeObject<IEnumerable<WorkOrderItemViewModel>>(responseContent);
            workOrderItems.Should().ContainSingle(woi => woi.Code == expectedNewCode);
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

        private async Task<int> ValidateWorkOrderCreation(HttpClient client, StringContent content, Action<WorkOrder> assertions, string uriString)
        {
            var response = await client.PostAsync(new Uri(uriString, UriKind.Relative), content);

            string responseContent = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK, responseContent);
            var id = JsonSerializer.Deserialize<int>(responseContent);

            using (var ctx = GetContext())
            {
                var db = ctx.DB;
                var repair = db.WorkOrders.Find(id);
                repair.Should().NotBeNull();
                assertions?.Invoke(repair);
            };

            return id;
        }

        public WorkOrder GetWorkOrderFromDB(int id)
        {
            using var ctx = GetContext();
            var db = ctx.DB;
            var repair = db.WorkOrders.Find(id);
            return repair;
        }
    }
}
