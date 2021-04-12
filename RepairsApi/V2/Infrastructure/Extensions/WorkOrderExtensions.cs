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
                WorkStatusCode.VariationApproved => WorkOrderStatus.VariationApproved,
                WorkStatusCode.VariationRejected => WorkOrderStatus.VariationRejected,
                _ => WorkOrderStatus.Unknown,
            };
        }

        public static string GetAction(this WorkOrder workOrder)
        {
            return workOrder.Reason switch
            {
                ReasonCode.PendingAuthorisation => WorkOrderReason.PendAuthorisation,
                ReasonCode.NoApproval => WorkOrderReason.Rejected,
                ReasonCode.Approved => WorkOrderReason.Approved,
                _ => WorkOrderReason.Unknown,
            };
        }
    }
}
