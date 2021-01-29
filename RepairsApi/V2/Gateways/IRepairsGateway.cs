using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public interface IRepairsGateway
    {
        Task<int> CreateWorkOrder(WorkOrder raiseRepair);
        Task<IEnumerable<WorkOrder>> GetWorkOrders(params Expression<Func<WorkOrder, bool>>[] whereExpressions);
        Task<WorkOrder?> GetWorkOrder(int id);
        Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(WorkOrder workOrder);
        Task AddWorkElement(int id, WorkElement workElement);
        Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(int id);
    }
}
