using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public class WorkOrderCompletionGateway : IWorkOrderCompletionGateway
    {
        public Task<int> CreateWorkOrderCompletion(WorkOrderComplete completion)
        {
            throw new System.NotImplementedException();
        }
    }
}
