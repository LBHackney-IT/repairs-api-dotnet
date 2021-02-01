using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public class ScheduleOfRatesGateway : IScheduleOfRatesGateway
    {
        private DbSet<ScheduleOfRates> SORCodes { get; }
        private DbSet<SORTrade> Trades { get; }
        private DbSet<SORContractor> Contractors { get; }

        public ScheduleOfRatesGateway(RepairsContext context)
        {
            SORCodes = context.SORCodes;
            Trades = context.Trades;
            Contractors = context.Contractors;
        }

        public async Task<IEnumerable<ScheduleOfRates>> GetSorCodes(string contractorRef, string tradeCode)
        {
            return await SORCodes.Where(sor => sor.SORContractorRef == contractorRef && sor.TradeCode == tradeCode).ToListAsync();
        }

        // TODO This can be a slow query. usages should be done in the gateway so a full queryable can be built for performance
        public async Task<IEnumerable<ScheduleOfRates>> GetSorCodes(string contractorRef)
        {
            return await SORCodes.Where(sor => sor.SORContractorRef == contractorRef).ToListAsync(); 
        }

        public async Task<IEnumerable<SORTrade>> GetTrades()
        {
            return await Trades.ToListAsync();
        }

        public async Task<IEnumerable<SORContractor>> GetContractors(string propRef)
        {
            List<SORContractor> contractors = await Contractors.ToListAsync();
            return contractors;
        }

        public Task<double> GetCost(string customCode)
        {
            return SORCodes.Where(sor => sor.CustomCode == customCode).Select(sor => sor.Cost).SingleAsync();
        }
    }
}
