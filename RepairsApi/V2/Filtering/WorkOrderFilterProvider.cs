using Microsoft.Extensions.Options;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Gateways;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Filtering
{
    public class WorkOrderFilterProvider : IFilterProvider
    {
        private readonly FilterConfiguration _options;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public WorkOrderFilterProvider(IOptions<FilterConfiguration> options, IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _options = options.Value;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task<ModelFilterConfiguration> GetFilter()
        {
            var filters = _options[FilterConstants.WorkOrder];

            var trades = await _scheduleOfRatesGateway.GetTrades();
            var liveContractors = await _scheduleOfRatesGateway.GetLiveContractors();

            filters[FilterSectionConstants.Trades] = new List<FilterOption>(trades.Select(t => new FilterOption { Key = t.Code, Description = t.Name }));
            filters[FilterSectionConstants.Contractors] = new List<FilterOption>(liveContractors.Select(c => new FilterOption { Key = c.ContractorReference, Description = c.ContractorName }));

            return filters;
        }
    }
}
