using NUnit.Framework;
using System;
using FluentAssertions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.Tests.V2.E2ETests
{
    public class HubUserApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task GetHubUser()
        {
            var client = CreateClient();
            client.SetAgent("raise150", "vary150");
            var response = await client.GetAsync(new Uri("/api/v2/hub-user", UriKind.Relative));

            var content = response.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<HubUserModel>(stringContent);

            response.StatusCode.Should().Be(200);
            convertedResponse.VaryLimit.Should().Be("150");
            convertedResponse.RaiseLimit.Should().Be("150");
        }
    }
}
