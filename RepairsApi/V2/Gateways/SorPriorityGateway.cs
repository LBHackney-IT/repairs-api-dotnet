using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public class SorPriorityGateway : ISorPriorityGateway
    {
        private readonly DbSet<SORPriority> _priorities;

        public SorPriorityGateway(RepairsContext context)
        {
            _priorities = context.SORPriorities;
        }

        public async Task<IEnumerable<SORPriority>> GetPriorities()
        {
            return await _priorities.ToListAsync();
        }
    }
}
