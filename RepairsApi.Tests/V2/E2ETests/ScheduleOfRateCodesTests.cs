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
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.Tests.V2.E2ETests
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
            var response = await client.GetAsync(new Uri("/api/v2/schedule-of-rates/trades", UriKind.Relative));

            response.StatusCode.Should().Be(200);

            var result = await GetResult<IEnumerable<SorTradeResponse>>(response);
            return result;
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
        public async Task GetObseleteCodes()
        {
            var client = CreateClient();

            var response = await client.GetAsync(new Uri($"/api/v2/schedule-of-rates/codes", UriKind.Relative));

            response.StatusCode.Should().Be(200);

            var result = await GetResult<IEnumerable<ScheduleOfRatesModel>>(response);

            result.Count().Should().BeGreaterOrEqualTo(1);
        }

        private static async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
