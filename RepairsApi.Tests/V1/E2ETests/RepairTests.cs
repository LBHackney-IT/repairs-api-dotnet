using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V1.Boundary;
using RepairsApi.V1.Generated;
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
    public class RepairTests : MockWebApplicationFactory
    {
        [Test]
        public async Task CreatesRepair()
        {
            var client = CreateClient();

            // Request is auto filling properties
            RaiseRepair request = new RaiseRepair
            {
                DescriptionOfWork = "test description"
            };
            StringContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

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
    }
}
