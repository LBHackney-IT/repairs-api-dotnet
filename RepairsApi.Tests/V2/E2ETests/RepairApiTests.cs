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

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Tests")]
    public class RepairApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task CreatesRepairFromJson()
        {
            var client = CreateClient();

            // These request need to pulled from json to stop c sharp adding in default enum properties
            string request = Requests.RaiseRepair;
            StringContent content = new StringContent(request, Encoding.UTF8, "application/json");

            await RaiseRepairAndValidate(client, content);
        }

        [Test]
        public async Task CreateRepairFromFullRequest()
        {
            var client = CreateClient();

            var request = GenerateWorkOrder<RaiseRepair>().Generate();
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            await RaiseRepairAndValidate(client, content, repair =>
            {
                repair.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
                repair.AgentName.Should().Be(TestUserInformation.NAME);
            });
        }

        [Test]
        public async Task ScheduleRepair()
        {
            var client = CreateClient();

            Generator<ScheduleRepair> generator;
            using (var ctx = GetContext())
            {
                generator = GenerateWorkOrder<ScheduleRepair>()
                    .AddValue(new List<double> { 1 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                    .WithValidCodes(ctx.DB);
            }

            var request = generator.Generate();
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            await ScheduleRepairAndValidate(client, content, repair =>
            {
                repair.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
            });
        }

        [Test]
        public async Task ScheduleReturns401WhenLimitExceeded()
        {
            var client = CreateClient();

            Generator<ScheduleRepair> generator = GenerateWorkOrder<ScheduleRepair>()
                .AddValue(new List<double>{ 1000 }, (RateScheduleItem rsi) => rsi.Quantity.Amount);

            var request = generator.Generate();
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(new Uri("/api/v2/repairs/schedule", UriKind.Relative), content);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ViewElements()
        {
            var client = CreateClient();

            Generator<ScheduleRepair> generator = GenerateWorkOrder<ScheduleRepair>();

            var request = generator.Generate();
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var woId = await ScheduleRepairAndValidate(client, content);

            var response = await client.GetAsync(new Uri($"/api/v2/repairs/{woId}/tasks", UriKind.Relative));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseContent = await response.Content.ReadAsStringAsync();
            var workOrders = JsonConvert.DeserializeObject<IEnumerable<WorkOrderItemViewModel>>(responseContent);

            int requestItemCount = request.WorkElement.Aggregate(0, (count, we) => count + we.RateScheduleItem.Count);
            workOrders.Should().HaveCount(requestItemCount);
        }

        [Test]
        public async Task BadRequestWhenMultipleAmountsProvided()
        {
            var client = CreateClient();

            var request = GenerateWorkOrder<RaiseRepair>().Generate();
            request.WorkElement.First().RateScheduleItem.First().Quantity.Amount.Add(3.5);
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(new Uri("/api/v2/repairs", UriKind.Relative), content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task BadRequestWhenDoesntHaveRequiredFromJson()
        {
            var client = CreateClient();
            client.SetAgent();
            string request = Requests.InvalidRaiseRepair;
            StringContent content = new StringContent(request, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(new Uri("/api/v2/repairs", UriKind.Relative), content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetListOfWorkOrders()
        {
            var client = CreateClient();

            var request = GenerateWorkOrder<RaiseRepair>().Generate();
            request.DescriptionOfWork = "expectedDescription";
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            await RaiseRepairAndValidate(client, content);

            var response = await client.GetAsync(new Uri("/api/v2/repairs", UriKind.Relative));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseContent = await response.Content.ReadAsStringAsync();
            var workOrders = JsonConvert.DeserializeObject<List<WorkOrderListItem>>(responseContent);
            workOrders.Should().Contain(wo => wo.Description == request.DescriptionOfWork);
        }

        [Test]
        public async Task CompleteRaiseRepairWorkOrder()
        {
            string endpoint = "/api/v2/repairs";

            Generator<RaiseRepair> requestGenerator = GenerateWorkOrder<RaiseRepair>();

            var request = requestGenerator.Generate();

            await ValidateCreationAndCompletion(request, endpoint);
        }

        [Test]
        public async Task CompleteScheduleRepairWorkOrder()
        {
            string endpoint = "/api/v2/repairs/schedule";
            Generator<ScheduleRepair> requestGenerator = GenerateWorkOrder<ScheduleRepair>();

            var request = requestGenerator.Generate();

            await ValidateCreationAndCompletion(request, endpoint);
        }

        [Test]
        public async Task CompleteRepairTwice()
        {
            string endpoint = "/api/v2/repairs/schedule";
            Generator<ScheduleRepair> requestGenerator = GenerateWorkOrder<ScheduleRepair>();

            var request = requestGenerator.Generate();
            var client = CreateClient();

            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var workOrderId = await ValidateWorkOrderCreation(client, content, null, endpoint);

            var response = await CompleteWorkOrder(client, workOrderId.ToString());
            var secondResponse = await CompleteWorkOrder(client, workOrderId.ToString());

            response.IsSuccessStatusCode.Should().BeTrue();
            secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateScheduleRepairWorkOrder()
        {
            const string expectedCode = "expectedCode";
            var (workOrderId, response) = await ScheduleAndUpdateWorkOrder(expectedCode, "comments", 0);
            await ValidateUpdate(response, workOrderId, expectedCode);
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
            const string expectedCode = "expectedCode";
            var (workOrderId, httpResponseMessage) = await ScheduleAndUpdateWorkOrder(expectedCode, expectedNote);
            await ValidateUpdate(httpResponseMessage, workOrderId, expectedCode);

            var client = CreateClient();
            var response = await client.GetAsync(new Uri($"/api/v2/repairs/{workOrderId}/notes", UriKind.Relative));

            response.StatusCode.Should().Be(HttpStatusCode.OK, response.Content.ToString());
            string responseContent = await response.Content.ReadAsStringAsync();
            var notes = JsonConvert.DeserializeObject<IList<NoteListItem>>(responseContent);
            notes.Should().ContainSingle(n => n.Note == expectedNote);
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

            var workOrderId = await ValidateWorkOrderCreation(client, content, null, endpoint);

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

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK, updateResponse.Content.ToString());

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
                                .AddValue(new List<double> { 0 }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                                .WithValidCodes(db);
            };

            return gen;
        }

        private async Task ValidateCreationAndCompletion(object request, string endpoint)
        {
            var client = CreateClient();

            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            await ValidateWorkOrderCreation(client, content, null, endpoint);

            var response = await client.GetAsync(new Uri("/api/v2/repairs", UriKind.Relative));
            string responseContent = await response.Content.ReadAsStringAsync();
            var workOrders = JsonConvert.DeserializeObject<List<WorkOrderListItem>>(responseContent);

            string workOrderId = workOrders.First().Reference.ToString();
            HttpResponseMessage completeResponse = await CompleteWorkOrder(client, workOrderId);

            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK, await completeResponse.Content.ReadAsStringAsync());
        }

        private static async Task<HttpResponseMessage> CompleteWorkOrder(HttpClient client, string workOrderId)
        {
            Generator<WorkOrderComplete> generator = new Generator<WorkOrderComplete>()
                .AddWorkOrderCompleteGenerators()
                .AddValue(workOrderId, (WorkOrderComplete woc) => woc.WorkOrderReference.ID)
                .AddValue(JobStatusUpdateTypeCode._0, (JobStatusUpdates jsu) => jsu.TypeCode)
                .AddValue(CustomJobStatusUpdates.CANCELLED, (JobStatusUpdates jsu) => jsu.OtherType);

            var completeRequest = generator.Generate();

            var serializedCompleteContent = JsonConvert.SerializeObject(completeRequest);
            StringContent completeContent = new StringContent(serializedCompleteContent, Encoding.UTF8, "application/json");
            var completeResponse = await client.PostAsync(new Uri("/api/v2/workOrderComplete", UriKind.Relative), completeContent);
            return completeResponse;
        }

        private async Task RaiseRepairAndValidate(HttpClient client, StringContent content, Action<WorkOrder> assertions = null)
        {
            const string uriString = "/api/v2/repairs";
            await ValidateWorkOrderCreation(client, content, assertions, uriString);
        }

        private async Task<int> ScheduleRepairAndValidate(HttpClient client, StringContent content, Action<WorkOrder> assertions = null)
        {
            const string uriString = "/api/v2/repairs/schedule";
            return await ValidateWorkOrderCreation(client, content, assertions, uriString);
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

    }
}
