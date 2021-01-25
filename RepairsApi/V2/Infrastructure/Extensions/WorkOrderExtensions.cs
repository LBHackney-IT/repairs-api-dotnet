using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.Infrastructure.Extensions
{
    public static class WorkOrderExtensions
    {
        public static string GetStatus(this WorkOrder workOrder)
        {
            return workOrder.WorkOrderComplete is null ? WorkOrderStatus.InProgress : WorkOrderStatus.Complete;
        }
    }
}
