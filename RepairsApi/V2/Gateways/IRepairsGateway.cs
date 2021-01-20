using System.Collections.Generic;
using System.Linq;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public interface IRepairsGateway
    {
        Task<int> CreateWorkOrder(WorkOrder raiseRepair);
        IEnumerable<WorkOrder> GetWorkOrders();
        WorkOrder? GetWorkOrder(int id);
        IEnumerable<WorkElement> GetWorkElementsForWorkOrder(WorkOrder workOrder);
    }
}
