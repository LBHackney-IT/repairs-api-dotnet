using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.Tests.E2ETests
{
    public class ScheduleOfRateCodesTests : MockWebApplicationFactory
    {
        [Test]
        public async Task SuccessfullyGetsTrades()
        {
            var client = CreateClient();

            IEnumerable<SorTradeResponse> result = await GetTrades(client);

            result.Should().NotBeEmpty();
        }

        private static async Task<IEnumerable<SorTradeResponse>> GetTrades(HttpClient client)
        {
            var response = await client.GetAsync(new Uri($"/api/v2/schedule-of-rates/trades?propref={TestDataSeeder.PropRef}", UriKind.Relative));

            response.StatusCode.Should().Be(200);

            var result = await GetResult<IEnumerable<SorTradeResponse>>(response);
            return result;
        }

        [Test]
        public async Task ListSORPriorities()
        {
            var client = CreateClient();

            var response = await client.GetAsync(new Uri("api/v2/schedule-of-rates/priorities", UriKind.Relative));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseContent = await response.Content.ReadAsStringAsync();
            var codes = JsonConvert.DeserializeObject<List<SORPriority>>(responseContent);
            codes.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task GetCodes()
        {
            var client = CreateClient();

            var propRef = TestDataSeeder.PropRef;
            var tradeCode = TestDataSeeder.Trade;
            var contractorRef = TestDataSeeder.Contractor;

            var response = await client.GetAsync(new Uri($"/api/v2/schedule-of-rates/codes?tradeCode={tradeCode}&propertyReference={propRef}&contractorReference={contractorRef}", UriKind.Relative));

            response.StatusCode.Should().Be(200);

            var result = await GetResult<IEnumerable<ScheduleOfRatesModel>>(response);

            result.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public async Task GetContractors()
        {
            var client = CreateClient();

            var propRef = TestDataSeeder.PropRef;
            var tradeCode = TestDataSeeder.Trade;

            var response = await client.GetAsync(new Uri($"/api/v2/contractors?tradeCode={tradeCode}&propertyReference={propRef}", UriKind.Relative));

            response.StatusCode.Should().Be(200);

            var result = await GetResult<IEnumerable<Contractor>>(response);

            result.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public async Task GetCode()
        {
            var client = CreateClient();
            client.SetGroups(GetGroup(TestDataSeeder.Contractor));

            var propRef = TestDataSeeder.PropRef;
            var contractorRef = TestDataSeeder.Contractor;
            var sorCode = TestDataSeeder.SorCode;

            var response = await client.GetAsync(new Uri($"/api/v2/schedule-of-rates/codes/{sorCode}?propertyReference={propRef}&contractorReference={contractorRef}", UriKind.Relative));

            response.StatusCode.Should().Be(200);

            var result = await GetResult<ScheduleOfRatesModel>(response);

            result.Should().NotBeNull();
        }


        [Test]
        public async Task GetCode404()
        {
            var client = CreateClient();
            client.SetGroups(GetGroup(TestDataSeeder.Contractor));

            var propRef = TestDataSeeder.PropRef;
            var contractorRef = TestDataSeeder.Contractor;

            var response = await client.GetAsync(new Uri($"/api/v2/schedule-of-rates/codes/1?propertyReference={propRef}&contractorReference={contractorRef}", UriKind.Relative));

            response.StatusCode.Should().Be(404);
        }

        [TestCase("/api/v2/contractors?tradeCode=1&propertyReference=1")]
        [TestCase("api/v2/schedule-of-rates/priorities")]
        public async Task AgentAuthorisationTests(string test)
        {
            await AuthorisationHelper.VerifyContractorUnauthorised(
                CreateClient(),
                GetGroup(TestDataSeeder.Contractor),
                async client =>
                {
                    return await client.GetAsync(new Uri(test, UriKind.Relative));
                });
        }

        [TestCase("/api/v2/schedule-of-rates/codes/1?propertyReference=1&contractorReference=1")]
        public async Task ContractorAuthorisationTests(string test)
        {
            await AuthorisationHelper.VerifyAgentUnauthorised(
                CreateClient(),
                async client =>
                {
                    return await client.GetAsync(new Uri(test, UriKind.Relative));
                });
        }

        private static async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
