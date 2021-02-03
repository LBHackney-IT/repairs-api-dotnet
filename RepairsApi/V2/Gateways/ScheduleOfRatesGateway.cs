using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
            var result = await
            (
                from sor in _context.SORCodes
                join sorContract in _context.SORContracts on sor.CustomCode equals sorContract.SorCodeCode
                join contract in _context.Contracts on sorContract.ContractReference equals contract.ContractReference
                // where
                //     sor.Trade.Code == tradeCode &&
                //     contract.PropertyMap.Any(pm => pm.PropRef == propertyReference) &&
                //     contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < contract.TerminationDate
                select new
                {
                    sor, sorContract, contract
                }
            ).ToListAsync();

            return from r in result
                group new
                {
                    r.contract, r.sorContract
                } by r.sor
                into contractGroup
                select new SorCodeResult
                {
                    Code = contractGroup.Key.CustomCode,
                    Description = contractGroup.Key.CustomName,
                    PriorityCode = contractGroup.Key.Priority.PriorityCode,
                    PriorityDescription = contractGroup.Key.Priority.Description,
                    Contracts = contractGroup
                        .Select(c => new SorCodeContractResult
                        {
                            ContractorCode = c.contract.Contractor.Reference,
                            ContractReference = c.contract.ContractReference,
                            ContractorName = c.contract.Contractor.Name,
                            ContractCost = c.sorContract.Cost
                        })
                };
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
