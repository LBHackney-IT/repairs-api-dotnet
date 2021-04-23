using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;
using V2_Generated_DRS;

namespace RepairsApi.V2.Services
{
    public interface IDrsService
    {
        Task OpenSession();
        Task<order> CreateOrder(WorkOrder workOrder);
        Task<bool> ContractorUsingDrs(string contractorRef);
    }

}
