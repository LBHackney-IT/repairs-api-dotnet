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
                            ContractorCode = c.Contract.Contractor.Reference,
                            ContractReference = c.Contract.ContractReference,
                            ContractorName = c.Contract.Contractor.Name,
                            ContractCost = c.Cost
                        })
                }).ToListAsync();
        }

        public async Task<double?> GetCost(string contractReference, string sorCode)
        {
            return await _context.SORContracts
                .Where(c => c.ContractReference == contractReference && c.SorCodeCode == sorCode)
                .Select(c => c.Cost).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetContracts(string contractorReference)
        {
            return await _context.Contracts
                .Where(c => c.Contractor.Reference == contractorReference)
                .Select(c => c.ContractReference).ToListAsync();
        }
    }
}
