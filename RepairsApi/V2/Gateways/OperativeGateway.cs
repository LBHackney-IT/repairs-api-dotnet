using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;


namespace RepairsApi.V2.Gateways
{
    public class OperativeGateway : IOperativeGateway
    {
        private readonly RepairsContext _context;

        public OperativeGateway(RepairsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Operative>> GetByFilterAsync(IFilter<Operative> filter)
        {
            var query = filter.Apply(_context.Operatives);
            return await query.ToListAsync();
        }

        public async Task<Operative> GetByPayrollNumberAsync(string operativePrn)
        {
            var query = _context.Operatives
                .Where(operative => operative.PayrollNumber == operativePrn);
            return await query.SingleAsync();
        }
    }
}
