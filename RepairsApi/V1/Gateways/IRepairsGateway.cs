using System.Collections.Generic;
using System.Linq;
using RepairsApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public interface IRepairsGateway
    {
        Task<int> CreateWorkOrder(WorkOrder raiseRepair);
        IEnumerable<WorkOrder> GetWorkOrders();
    }
}
