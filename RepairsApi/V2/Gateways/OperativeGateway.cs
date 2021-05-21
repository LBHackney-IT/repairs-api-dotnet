using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Operative>> GetByQueryAsync(Boundary.Request.OperativeRequest searchModel)
        {
            var query = _context.Operatives
                .Where(operative => searchModel.Id == null || operative.Id == searchModel.Id)
                .Where(operative => searchModel.Name == null || operative.Person.Name.Full.Contains(searchModel.Name))
                .Where(operative => searchModel.Trade == null || operative.Trade.Any(trade => trade.CustomName == searchModel.Trade));

            return await query.ToListAsync();
        }
    }
}
