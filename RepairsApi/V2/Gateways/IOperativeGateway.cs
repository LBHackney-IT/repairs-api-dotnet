using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IOperativeGateway
    {
        Task<IEnumerable<Operative>> GetByFilterAsync(IFilter<Operative> filter);
    }
}
