using Microsoft.Extensions.Options;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Filtering
{
    public class WorkOrderFilterProvider : IFilterProvider
    {
        private readonly FilterConfiguration _options;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;
        private readonly ICurrentUserService _currentUserService;

        public WorkOrderFilterProvider(IOptions<FilterConfiguration> options,
            IScheduleOfRatesGateway scheduleOfRatesGateway,
            ICurrentUserService currentUserService
            )
        {
            _options = options.Value;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
            _currentUserService = currentUserService;
        }

        public async Task<ModelFilterConfiguration> GetFilter()
        {
            var filters = _options[FilterConstants.WorkOrder];

            var trades = await _scheduleOfRatesGateway.GetTrades();

            filters[FilterSectionConstants.Trades] = new List<FilterOption>(trades.Select(t => new FilterOption { Key = t.Code, Description = t.Name }));

            filters[FilterSectionConstants.Contractors] = await GetContractors();

            return filters;
        }

        private async Task<List<FilterOption>> GetContractors()
        {
            var liveContractors = await _scheduleOfRatesGateway.GetLiveContractors();
            var filterOptions = new List<FilterOption>(liveContractors.Select(c => new FilterOption { Key = c.ContractorReference, Description = c.ContractorName }));

            if (!_currentUserService.HasAnyGroup(UserGroups.Agent, UserGroups.AuthorisationManager, UserGroups.ContractManager))
            {
                var groups = _currentUserService.GetContractors();

                return filterOptions.Where(fo => groups.Contains(fo.Key)).ToList();
            }

            return filterOptions;
        }
    }
}
