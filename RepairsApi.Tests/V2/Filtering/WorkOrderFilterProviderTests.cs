using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Gateways;
using System.Threading.Tasks;
using SorCodeTrade = RepairsApi.V2.Infrastructure.Hackney.SorCodeTrade;

namespace RepairsApi.Tests.V2.Filtering
{
    public class WorkOrderFilterProviderTests
    {
        [Test]
        public async Task SendsFilter()
        {
            const string ModelName = FilterConstants.WorkOrder;
            const string FilterName = "testFilter";
            const string FilterOptionKey = "1";
            const string FilterOptionValue = "Option1";
            const string ContractorReference = "cont_ref";
            const string ContractorName = "cont_name";
            const string TradeCode = "trade_code";
            const string TradeName = "trade_name";

            IOptions<FilterConfiguration> options = CreateFilterConfiguration(ModelName, FilterName, FilterOptionKey, FilterOptionValue);
            var mockScheduleOfRatesGateway = new Mock<IScheduleOfRatesGateway>();
            mockScheduleOfRatesGateway.Setup(m => m.GetLiveContractors()).ReturnsAsync(new Contractor(ContractorReference, ContractorName).MakeArray());
            mockScheduleOfRatesGateway.Setup(m => m.GetTrades()).ReturnsAsync(new SorCodeTrade(TradeCode, TradeName).MakeArray());

            var sut = new WorkOrderFilterProvider(options, mockScheduleOfRatesGateway.Object);

            var result = await sut.GetFilter();

            ValidateFilterSection(result, FilterName, FilterOptionKey, FilterOptionValue);
            ValidateFilterSection(result, FilterSectionConstants.Contractors, ContractorReference, ContractorName);
            ValidateFilterSection(result, FilterSectionConstants.Trades, TradeCode, TradeName);
        }

        private static void ValidateFilterSection(ModelFilterConfiguration result, string filterName, string filterOptionKey, string filterOptionValue)
        {
            result.Should().ContainKey(filterName);
            var filters = result[filterName];

            filters.Should().ContainSingle(option => option.Key == filterOptionKey && option.Description == filterOptionValue);
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
