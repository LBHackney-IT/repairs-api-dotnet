using FluentAssertions;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using RepairsApi.V1.Infrastructure;

namespace RepairsApi.Tests.V1.E2ETests
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

        private async Task RaiseRepairAndValidate(HttpClient client, StringContent content, Action<WorkOrder> assertions = null)
        {
            var response = await client.PostAsync(new Uri("/api/v2/repairs", UriKind.Relative), content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            string responseContent = await response.Content.ReadAsStringAsync();
            var id = JsonSerializer.Deserialize<int>(responseContent);

            WithContext(repairsContext =>
            {
                var repair = repairsContext.WorkOrders.Find(id);
                repair.Should().NotBeNull();
                assertions?.Invoke(repair);
            });
        }

    }
}
