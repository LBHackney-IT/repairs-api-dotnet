using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IOperativesGateway
    {
        Task<IEnumerable<Operative>> ListByFilterAsync(IFilter<Operative> filter);
        Task<Operative> GetAsync(string operativePrn);
        Task ArchiveAsync(string operativePrn);
        Task AssignOperatives(int workOrderId, OperativeAssignmentType type, params int[] operativeIds);
    }
}
