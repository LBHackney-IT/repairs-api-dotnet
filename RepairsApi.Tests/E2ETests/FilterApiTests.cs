using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.Tests.E2ETests
{
    public class FilterApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task GetWorkOrderFilters()
        {
            var (code, result) = await Get<Dictionary<string, List<FilterOption>>>("/api/v2/filter/WorkOrder");

            code.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeEmpty();
            result[FilterSectionConstants.Trades].Should().NotBeEmpty();
            result[FilterSectionConstants.Contractors].Should().NotBeEmpty();
        }

        [Test]
        public async Task GetUserContractorFilters()
        {
            SetUserRole(TestDataSeeder.DRSContractor);
            var (code, result) = await Get<Dictionary<string, List<FilterOption>>>("/api/v2/filter/WorkOrder");

            code.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeEmpty();
            result[FilterSectionConstants.Contractors].Should().HaveCount(1);
            result[FilterSectionConstants.Contractors].Should().ContainSingle(fo => fo.Key == TestDataSeeder.DRSContractor && fo.Description == TestDataSeeder.DRSContractor);
        }
    }
}
