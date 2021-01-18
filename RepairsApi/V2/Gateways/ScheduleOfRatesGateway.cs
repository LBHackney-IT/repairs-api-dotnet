using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public class ScheduleOfRatesGateway : IScheduleOfRatesGateway
    {
        private readonly RepairsContext _context;

        public ScheduleOfRatesGateway(RepairsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleOfRates>> GetSorCodes()
        {
            return await _context.SORCodes.ToListAsync();
        }
    }
}
