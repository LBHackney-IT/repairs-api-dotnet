using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Factories;
using Contractor = RepairsApi.V2.Domain.Contractor;

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

        public async Task<IEnumerable<ScheduleOfRatesModel>> GetSorCodes(string propertyReference, string tradeCode, string contractorReference)
        {
            return await
            (
                from sor in _context.SORCodes
                join sorContract in _context.SORContracts on sor.CustomCode equals sorContract.SorCodeCode
                join contract in _context.Contracts on sorContract.ContractReference equals contract.ContractReference
                where
                sor.Trade.Code == tradeCode &&
                contract.PropertyMap.Any(pm => pm.PropRef == propertyReference) &&
                contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < contract.TerminationDate &&
                contract.ContractorReference == contractorReference
                select new ScheduleOfRatesModel
                {
                    CustomCode = sor.CustomCode,
                    CustomName = sor.CustomName,
                    Priority = new Domain.SORPriority
                    {
                        Description = sor.Priority.Description,
                        PriorityCode = sor.Priority.PriorityCode
                    }
                }
            ).ToListAsync();
        }

        public async Task<double?> GetCost(string contractorReference, string sorCode)
        {
            return await _context.SORContracts
                .Where(c => c.Contract.ContractorReference == contractorReference && c.SorCodeCode == sorCode)
                .Select(c => c.Cost).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetContracts(string contractorReference)
        {
            return await _context.Contracts
                .Where(c => c.Contractor.Reference == contractorReference)
                .Select(c => c.ContractReference).ToListAsync();
        }

        public async Task<IEnumerable<ScheduleOfRatesModel>> GetSorCodes()
        {
            return await _context.SORCodes
                .Select(sor => new ScheduleOfRatesModel
                {
                    CustomCode = sor.CustomCode,
                    CustomName = sor.CustomName,
                    Priority = new Domain.SORPriority
                    {
                        Description = sor.Priority.Description,
                        PriorityCode = sor.Priority.PriorityCode
                    }
                }).ToListAsync();
        }

        public async Task<IEnumerable<Contractor>> GetContractors(string propertyRef, string tradeCode)
        {
            var contractors = _context.Contracts.Where(contract =>
                contract.SorCodeMap.Any(scm =>
                    scm.SorCode.TradeCode == tradeCode
                ) &&
                contract.PropertyMap.Any(pm =>
                    pm.PropRef == propertyRef
                )
            ).Select(c => new Contractor
            {
                ContractorName = c.Contractor.Name,
                ContractorReference = c.Contractor.Reference
            });

            return await contractors.ToListAsync();
        }
    }
}
