using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure.Hackney;

namespace RepairsApi.V2.Gateways
{
    public interface ISorPriorityGateway
    {
        Task<IEnumerable<SORPriority>> GetPriorities();
        Task<char> GetLegacyPriorityCode(int priorityCode);
    }

}
