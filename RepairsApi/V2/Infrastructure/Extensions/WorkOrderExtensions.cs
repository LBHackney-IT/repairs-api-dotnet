using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.Infrastructure.Extensions
{
    public static class WorkOrderExtensions
    {
        public static string GetStatus(this WorkOrder workOrder)
        {
            switch (workOrder.StatusCode)
            {
                case WorkStatusCode.Open: return WorkOrderStatus.InProgress;
                case WorkStatusCode.Complete: return WorkOrderStatus.Complete;
                case WorkStatusCode.Canceled: return WorkOrderStatus.Cancelled;
                default: return WorkOrderStatus.Unknown;
            }
        }
    }
}
