using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.Infrastructure.Extensions
{
    public static class WorkOrderExtensions
    {
        public static string GetStatus(this WorkOrder workOrder)
        {
            switch (workOrder.StatusCode)
            {
                case WorkStatusCode.Open: return WorkOrderStatusResp.InProgress;
                case WorkStatusCode.Complete: return WorkOrderStatusResp.Complete;
                case WorkStatusCode.Canceled: return WorkOrderStatusResp.Cancelled;
                default: return WorkOrderStatusResp.Unknown;
            }
        }
    }
}
