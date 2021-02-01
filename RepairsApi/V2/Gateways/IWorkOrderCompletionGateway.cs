using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IWorkOrderCompletionGateway
    {
        Task<int> CreateWorkOrderCompletion(WorkOrderComplete completion);
        Task<bool> IsWorkOrderCompleted(int id);
    }

}
