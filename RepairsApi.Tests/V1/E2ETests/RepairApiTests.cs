using FluentAssertions;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V1.E2ETests
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Tests")]
    public class RepairApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task CreatesRepair()
        {
            var client = CreateClient();

            // These request need to pulled from json to stop c sharp adding in default enum properties
            string request = Requests.RaiseRepair;
            StringContent content = new StringContent(request, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(new Uri("/api/v2/repairs", UriKind.Relative), content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            string responseContent = await response.Content.ReadAsStringAsync();
            var id = JsonSerializer.Deserialize<int>(responseContent);

            WithContext(repairsContext =>
            {
                var repair = repairsContext.WorkOrders.Find(id);
                repair.Should().NotBeNull();
            });
        }

        [Test]
        public async Task BadRequestWhenDoesntHaveRequired()
        {
            var client = CreateClient();

            string request = Requests.InvalidRaiseRepair;
            StringContent content = new StringContent(request, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(new Uri("/api/v2/repairs", UriKind.Relative), content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
