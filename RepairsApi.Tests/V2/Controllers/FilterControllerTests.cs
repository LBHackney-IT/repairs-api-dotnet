using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
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
            const string ModelName = "testModel";
            const string FilterName = "testFilter";
            const string FilterOptionKey = "1";
            const string FilterOptionValue = "Option1";

            IOptions<FilterConfiguration> options = CreateFilterConfiguration(ModelName, FilterName, FilterOptionKey, FilterOptionValue);
            var sut = new FilterController(options);

            var result = sut.GetFilterInformation(ModelName);

            GetStatusCode(result).Should().Be(200);

            var response = GetResultData<Dictionary<string, List<FilterOption>>>(result);
            response.Should().ContainKey(FilterName);
            var filters = response[FilterName];

            filters.Should().ContainSingle(option => option.Key == FilterOptionKey && option.Description == FilterOptionValue);
        }

        private static IOptions<FilterConfiguration> CreateFilterConfiguration(
            string modelName,
            string filterName,
            string filterOptionKey,
            string filterOptionValue)
        {
            var filterBuilder = new FilterConfigurationBuilder()
                .AddModel(modelName, builder =>
                {
                    builder.AddFilter(filterName, options =>
                    {
                        options.AddOption(filterOptionKey, filterOptionValue);
                    });
                });

            var options = Options.Create(filterBuilder.Build());
            return options;
        }
    }
}
