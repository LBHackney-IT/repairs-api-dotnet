using FluentAssertions;
using RepairsApi.V2.Authorisation;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RepairsApi.Tests.Helpers
{
    public static class AuthorisationHelper
    {
        public static async Task VerifyAgentUnauthorised(HttpClient client, Func<HttpClient, Task<HttpResponseMessage>> request)
        {
            client.SetAgent();
            var response = await request(client);

            response.StatusCode.Should().Be(401);
        }

        public static async Task VerifyContractorUnauthorised(HttpClient client, string group, Func<HttpClient, Task<HttpResponseMessage>> request)
        {
            client.SetGroups(group);
            var response = await request(client);

            response.StatusCode.Should().Be(401);
        }
    }
}
