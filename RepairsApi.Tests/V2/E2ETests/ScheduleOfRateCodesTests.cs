using System;
using System.Collections.Generic;
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
        public async Task ListSORCodes()
        {
            var client = CreateClient();

            var response = await client.GetAsync(new Uri("api/v2/schedule-of-rates/codes", UriKind.Relative));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            string responseContent = await response.Content.ReadAsStringAsync();
            var codes = JsonConvert.DeserializeObject<List<ScheduleOfRatesModel>>(responseContent);
            codes.Should().NotBeNullOrEmpty();
        }
    }
}
