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
                WorkStatusCode.PendApp => WorkOrderStatus.PendApp,
                WorkStatusCode.PendMaterial => WorkOrderStatus.PendMaterial,
                _ => WorkOrderStatus.Unknown,
            };
        }

        public static string GetAction(this WorkOrder workOrder)
        {
            return workOrder.Reason switch
            {
                //ReasonCode.NoBudget  
                //ReasonCode.LowPriority 
                //ReasonCode.FullyFunded 
                //ReasonCode.PartiallyFunded 
                //ReasonCode.ScheduleConflict 
                ReasonCode.NoApproval => WorkOrderReason.Rejected,
                //ReasonCode.Approved
                //ReasonCode.Priority 
                _ => WorkOrderReason.Unknown,
            };
        }
    }
}
