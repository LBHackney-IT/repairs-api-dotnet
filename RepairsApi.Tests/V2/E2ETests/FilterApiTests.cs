using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Controllers
{
    public class FilterApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task GetWorkOrderFilters()
        {
            var (code, result) = await Get<Dictionary<string, List<FilterOption>>>("/api/v2/filter/WorkOrder");

            code.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeEmpty();
            result["Trades"].Should().NotBeEmpty();
            result["Contractors"].Should().NotBeEmpty();
        }
    }
}
