using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using JsonSerializer = System.Text.Json.JsonSerializer;
using RepairsApi.V2.Infrastructure;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;
using RepairsApi.Tests.Helpers.StubGeneration;

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

            var request = RepairMockBuilder.CreateFullRaiseRepair();
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            await RaiseRepairAndValidate(client, content, repair =>
            {
                repair.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
            });
        }

        [Test]
        public async Task ScheduleRepair()
        {
            var client = CreateClient();

            Generator<ScheduleRepair> generator = new Generator<ScheduleRepair>()
                .AddWorkOrderGenerators();

            var request = generator.Generate();
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            await ScheduleRepairAndValidate(client, content, repair =>
            {
                repair.WorkPriority.NumberOfDays.Should().Be(request.Priority.NumberOfDays);
            });
        }

        [Test]
        public async Task ViewElements()
        {
            var client = CreateClient();

            Generator<ScheduleRepair> generator = new Generator<ScheduleRepair>()
                .AddWorkOrderGenerators();

            var request = generator.Generate();
            var serializedContent = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var woId = await ScheduleRepairAndValidate(client, content);

            var response = await client.GetAsync(new Uri($"/api/v2/repairs/{woId}/tasks", UriKind.Relative));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task BadRequestWhenMultipleAmountsProvided()
        {
            var client = CreateClient();

            var request = RepairMockBuilder.CreateFullRaiseRepair();
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

            string request = Requests.InvalidRaiseRepair;
            StringContent content = new StringContent(request, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(new Uri("/api/v2/repairs", UriKind.Relative), content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetListOfWorkOrders()
        {
            var client = CreateClient();

            var request = RepairMockBuilder.CreateFullRaiseRepair();
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

            Generator<RaiseRepair> requestGenerator = new Generator<RaiseRepair>()
                .AddWorkOrderGenerators();

            var request = requestGenerator.Generate();

            await ValidateCreationAndCompletion(request, endpoint);
        }

        [Test]
        public async Task CompleteScheduleRepairWorkOrder()
        {
            string endpoint = "/api/v2/repairs/schedule";

            Generator<ScheduleRepair> requestGenerator = new Generator<ScheduleRepair>()
                .AddWorkOrderGenerators();

            var request = requestGenerator.Generate();

            await ValidateCreationAndCompletion(request, endpoint);
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

            Generator<WorkOrderComplete> generator = new Generator<WorkOrderComplete>()
                .AddWorkOrderCompleteGenerators()
                .AddValue(workOrders.First().Reference.ToString(), (WorkOrderComplete woc) => woc.WorkOrderReference.ID);

            var completeRequest = generator.Generate();

            var serializedCompleteContent = JsonConvert.SerializeObject(completeRequest);
            StringContent completeContent = new StringContent(serializedCompleteContent, Encoding.UTF8, "application/json");
            var completeResponse = await client.PostAsync(new Uri("/api/v2/workOrderComplete", UriKind.Relative), completeContent);

            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK, completeResponse.Content.ToString());
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var id = JsonSerializer.Deserialize<int>(responseContent);

            WithContext(repairsContext =>
            {
                var repair = repairsContext.WorkOrders.Find(id);
                repair.Should().NotBeNull();
                assertions?.Invoke(repair);
            });

            return id;
        }
    }
}
