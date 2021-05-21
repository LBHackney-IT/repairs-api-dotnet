using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IOperativeGateway
    {
        Task<IEnumerable<Operative>> GetByQueryAsync(Boundary.Request.OperativeRequest searchModel);
    }
}
