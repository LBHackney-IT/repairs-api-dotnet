using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Controllers
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Tests")]
    public class FilterControllerTests : ControllerTests
    {
        [Test]
        public void SendsFilter()
        {
            var testModelFilter = new Dictionary<string, List<FilterOption>>
            {
                {
                    "testFilter",
                    new List<FilterOption>()
                    {
                        new FilterOption
                        {
                            Key = "1",
                            Description = "Option1"
                        }
                    }
                }
            };

            var options = Options.Create(new FilterConfiguration
            {
                { "testModel", testModelFilter }
            });

            var sut = new FilterController(options);

            var result = sut.GetFilterInformation("testModel");

            GetStatusCode(result).Should().Be(200);
            var response = GetResultData<Dictionary<string, List<FilterOption>>>(result);

            response.Should().ContainKey("testFilter");
            var filters = response["testFilter"];

        }
    }
}
