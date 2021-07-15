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
                WorkStatusCode.VariationPendingApproval => WorkOrderStatus.VariationPendingApproval,
                WorkStatusCode.PendMaterial => WorkOrderStatus.PendMaterial,
                WorkStatusCode.PendingApproval => WorkOrderStatus.PendingApproval,
                WorkStatusCode.VariationApproved => WorkOrderStatus.VariationApproved,
                WorkStatusCode.VariationRejected => WorkOrderStatus.VariationRejected,
                WorkStatusCode.NoAccess => WorkOrderStatus.NoAccess,
                WorkStatusCode.Locked => WorkOrderStatus.Locked,
                _ => WorkOrderStatus.Unknown,
            };
        }

        public static string GetAction(this WorkOrder workOrder)
        {
            return workOrder.Reason switch
            {
                ReasonCode.VariationPendingAuthorisation => WorkOrderReason.VariationPendingAuthorisation,
                ReasonCode.NoApproval => WorkOrderReason.Rejected,
                ReasonCode.Approved => WorkOrderReason.Approved,
                _ => WorkOrderReason.Unknown,
            };
        }
    }
}
