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

        public ScheduleOfRatesGateway(RepairsContext context)
        {
            SORCodes = context.SORCodes;
        }

        public Task<string> GetContractorReference(string customCode)
        {
            return SORCodes
                .Where(sor => sor.CustomCode == customCode)
                .Select(sor => sor.SORContractorRef)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ScheduleOfRates>> GetSorCodes(string contractorRef = null)
        {
            if (contractorRef.IsNullOrEmpty())
            {
                return await SORCodes.ToListAsync();
            }
            return await SORCodes.Where(sor => sor.SORContractorRef == contractorRef).ToListAsync();
        }
    }
}
