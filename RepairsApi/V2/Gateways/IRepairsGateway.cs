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
        Task<IEnumerable<WorkOrder>> GetWorkOrders();
        Task<IEnumerable<WorkOrder>> GetWorkOrders(Expression<Func<WorkOrder, bool>> whereExpression);
        Task<WorkOrder?> GetWorkOrder(int id);
    }
}
