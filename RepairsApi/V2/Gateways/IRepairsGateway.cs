using RepairsApi.V2.Filtering;
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
        Task<IEnumerable<WorkOrder>> GetWorkOrders(IFilter<WorkOrder> filter);
        Task<WorkOrder> GetWorkOrder(int id);
        Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(WorkOrder workOrder);
        Task<IEnumerable<WorkElement>> GetWorkElementsForWorkOrder(int id);
        Task UpdateWorkOrderStatus(int workOrderId, WorkStatusCode canceled);
        Task SaveChangesAsync();
        Task<IEnumerable<WorkOrder>> GetWorkOrders(params Expression<Func<WorkOrder, bool>>[] whereExpressions);
    }
}
