using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface ISorPriorityGateway
    {
        Task<IEnumerable<SORPriority>> GetPriorities();
    }

}
