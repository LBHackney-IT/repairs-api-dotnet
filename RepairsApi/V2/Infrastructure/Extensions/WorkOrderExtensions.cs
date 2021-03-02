using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.Infrastructure.Extensions
{
    public static class WorkOrderExtensions
    {
        public static string GetStatus(this WorkOrder workOrder)
        {
            return workOrder.StatusCode switch
            {
                WorkStatusCode.Open => WorkOrderStatus.InProgress,
                WorkStatusCode.Complete => WorkOrderStatus.Complete,
                WorkStatusCode.Canceled => WorkOrderStatus.Cancelled,
                WorkStatusCode.Hold => WorkOrderStatus.Hold,
                _ => WorkOrderStatus.Unknown,
            };
        }
    }
}
