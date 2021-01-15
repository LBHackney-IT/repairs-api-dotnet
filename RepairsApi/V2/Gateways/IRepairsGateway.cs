using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
    public interface IRepairsGateway
    {
        Task<int> CreateWorkOrder(WorkOrder raiseRepair);
        IEnumerable<WorkOrder> GetWorkOrders();
        Task<WorkOrder> GetWorkOrder(int id);
    }
}
