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
using RepairsApi.V2.Exceptions;

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
            return await (
                    from trade in _context.Trades
                    where (
                         from sor in _context.SORCodes
                         join sorContract in _context.SORContracts on sor.Code equals sorContract.SorCodeCode
                         join contract in _context.Contracts on sorContract.ContractReference equals contract.ContractReference
                         where
                        contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < contract.TerminationDate &&
                        sor.Enabled
                         select sor.TradeCode
                    ).Contains(trade.Code)

                    select trade
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<SorCodeTrade>> GetTrades(string propRef)
        {
            return await (
                    from trade in _context.Trades
                    where (
                         from sor in _context.SORCodes
                         join sorContract in _context.SORContracts on sor.Code equals sorContract.SorCodeCode
                         join contract in _context.Contracts on sorContract.ContractReference equals contract.ContractReference
                         where
                        contract.PropertyMap.Any(pm => pm.PropRef == propRef) &&
                        contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < contract.TerminationDate &&
                        sor.Enabled
                         select sor.TradeCode
                    ).Contains(trade.Code)

                    select trade
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleOfRatesModel>> GetSorCodes(string propertyReference, string tradeCode, string contractorReference)
        {
            return await
            (
                from sor in _context.SORCodes
                join sorContract in _context.SORContracts on sor.Code equals sorContract.SorCodeCode
                join contract in _context.Contracts on sorContract.ContractReference equals contract.ContractReference
                where
                sor.Trade.Code == tradeCode &&
                contract.PropertyMap.Any(pm => pm.PropRef == propertyReference) &&
                contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < contract.TerminationDate &&
                contract.ContractorReference == contractorReference &&
                sor.Enabled
                select new ScheduleOfRatesModel
                {
                    Code = sor.Code,
                    ShortDescription = sor.ShortDescription,
                    LongDescription = sor.LongDescription,
                    Priority = new Domain.SORPriority
                    {
                        Description = sor.Priority.Description,
                        PriorityCode = sor.Priority.PriorityCode
                    },
                    Cost = sorContract.Cost ?? sor.Cost
                }
            ).ToListAsync();
        }

        public async Task<double> GetCost(string contractorReference, string sorCode)
        {
            if (contractorReference is null) throw new ArgumentNullException(nameof(contractorReference));
            if (sorCode is null) throw new ArgumentNullException(nameof(sorCode));

            var today = DateTime.UtcNow.Date;

            var costs = await _context.SORContracts
                            .Where(c => c.Contract.ContractorReference == contractorReference && c.SorCodeCode == sorCode)
                            .Where(c => c.Contract.TerminationDate > today && c.Contract.EffectiveDate <= today)
                            .Select(c => new { ContractCost = c.Cost, CodeCost = c.SorCode.Cost }).FirstOrDefaultAsync();
            double? finalCost = costs?.ContractCost ?? costs?.CodeCost;

            if (!finalCost.HasValue) throw new ResourceNotFoundException($"Cannot find cost for code {sorCode}");

            return finalCost.Value;
        }

        public async Task<IEnumerable<string>> GetContracts(string contractorReference)
        {
            return await _context.Contracts
                .Where(c => c.Contractor.Reference == contractorReference)
                .Select(c => c.ContractReference).ToListAsync();
        }

        public Task<IEnumerable<Contractor>> GetLiveContractors()
        {
            var contractors = _context.Contractors.Where(c => _context.SecurityGroups.Any(sg => sg.ContractorReference == c.Reference))
                .Select(c => new Contractor
                {
                    ContractorName = c.Name,
                    ContractorReference = c.Reference
                });
            return Task.FromResult(contractors.AsEnumerable());
        }

        public async Task<IEnumerable<Contractor>> GetContractors(string propertyRef, string tradeCode)
        {
            var contractors = _context.Contracts.Where(contract =>
                contract.SorCodeMap.Any(scm =>
                    scm.SorCode.TradeCode == tradeCode && scm.SorCode.Enabled
                ) &&
                contract.PropertyMap.Any(pm =>
                    pm.PropRef == propertyRef
                ) &&
                contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < contract.TerminationDate
            ).Select(c => new Contractor
            {
                ContractorName = c.Contractor.Name,
                ContractorReference = c.Contractor.Reference
            });

            return await contractors.ToListAsync();
        }

        public async Task<Contractor> GetContractor(string contractorReference)
        {
            var contractor = await _context.Contractors.SingleOrDefaultAsync(c => c.Reference == contractorReference);

            if (contractor is null) return null;

            return new Contractor
            {
                ContractorName = contractor.Name,
                ContractorReference = contractor.Reference,
                UseExternalScheduleManager = contractor.UseExternalScheduleManager
            };
        }

        public async Task<ScheduleOfRatesModel> GetCode(string sorCode, string propertyReference, string contractorReference)
        {
            var model = await
            (
                from sor in _context.SORCodes
                join sorContract in _context.SORContracts on sor.Code equals sorContract.SorCodeCode
                join contract in _context.Contracts on sorContract.ContractReference equals contract.ContractReference
                where
                sor.Code == sorCode &&
                contract.PropertyMap.Any(pm => pm.PropRef == propertyReference) &&
                contract.EffectiveDate < DateTime.UtcNow && DateTime.UtcNow < contract.TerminationDate &&
                contract.ContractorReference == contractorReference
                select new ScheduleOfRatesModel
                {
                    Code = sor.Code,
                    ShortDescription = sor.ShortDescription,
                    LongDescription = sor.LongDescription,
                    Priority = new Domain.SORPriority
                    {
                        Description = sor.Priority.Description,
                        PriorityCode = sor.Priority.PriorityCode
                    },
                    Cost = sorContract.Cost ?? sor.Cost,
                    TradeCode = sor.TradeCode,
                    StandardMinuteValue = sor.StandardMinuteValue
                }
            ).ToListAsync();

            if (model.Count == 0) throw new ResourceNotFoundException("Could not find SOR code");
            if (model.Count > 1) throw new NotSupportedException("Multiple Valid Contracts found for code");

            return model.First();
        }
    }
}
