using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public class WorkOrderCompletionGateway : IWorkOrderCompletionGateway
    {
        private readonly RepairsContext _repairsContext;

        public WorkOrderCompletionGateway(RepairsContext repairsContext)
        {
            _repairsContext = repairsContext;
        }

        public async Task<bool> IsWorkOrderCompleted(int id)
        {
            return await _repairsContext.WorkOrderCompletes.AnyAsync(woc => woc.WorkOrder.Id == id);
        }

        public async Task<int> CreateWorkOrderCompletion(WorkOrderComplete completion)
        {
            _repairsContext.WorkOrderCompletes.Add(completion);
            await _repairsContext.SaveChangesAsync();

            return completion.Id;
        }
    }
}
