using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
    public class ScheduleOfRatesGateway : IScheduleOfRatesGateway
    {
        private readonly RepairsContext _context;

        public ScheduleOfRatesGateway(RepairsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SorCodeTrade>> GetTrades()
        {
            return await _context.Trades.ToListAsync();
        }

        public async Task<IEnumerable<SorCodeResult>> GetSorCodes(string propertyReference, string tradeCode)
        {
            return await _context.SORCodes
                .Where(sor => sor.Trade.Code == tradeCode)
                .Select(sor => new SorCodeResult
                {
                    Code = sor.CustomCode,
                    Description = sor.CustomName,
                    PriorityCode = sor.Priority.PriorityCode,
                    PriorityDescription = sor.Priority.Description,
                    Contracts = sor.SorCodeMap
                        .Where(c => c.Contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < c.Contract.TerminationDate)
                        .Where(c => c.Contract.PropertyMap.Any(pm => pm.PropRef == propertyReference))
                        .Select(c => new SorCodeContractResult
                        {
                            ContractorCode = c.Contract.Contractor.Code,
                            ContractReference = c.Contract.ContractReference,
                            ContractorName = c.Contract.Contractor.Name,
                            ContractCost = c.Cost
                        })
                }).ToListAsync();
        }

        public async Task<double?> GetCost(string customCode)
        {
            return await Task.FromResult(0.0);
        }

        public Task GetSorCodes(string contractorReference)
        {
            throw new NotImplementedException();
        }
    }
}
