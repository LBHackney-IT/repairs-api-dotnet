using RepairsApi.V2.Infrastructure;
using System;

namespace RepairsApi.V2.Helpers
{
    public static class WorkOrderExtensions
    {
        public static void VerifyCanBookAppointment(this WorkOrder wo)
        {
            if (wo.StatusCode == WorkStatusCode.PendingApproval) throw new NotSupportedException("Cannot book an appointment on an unapproved WO");
        }

        public static void VerifyCanComplete(this WorkOrder wo)
        {
            if (wo.StatusCode == WorkStatusCode.PendingApproval) throw new NotSupportedException("Work Orders pending approval can not be completed.");
            if (wo.StatusCode == WorkStatusCode.PendingVariation) throw new NotSupportedException("Work Orders with outstanding variations can not be completed.");
        }

        public static void VerifyCanResumeJob(this WorkOrder wo)
        {
            if (
                wo.StatusCode != WorkStatusCode.PendMaterial &&
                wo.StatusCode != WorkStatusCode.Hold
            )
                throw new NotSupportedException("Cannot Resume Job");
        }

        public static void VerifyCanVary(this WorkOrder wo)
        {
            if (wo.StatusCode == WorkStatusCode.PendingVariation ||
                wo.StatusCode == WorkStatusCode.PendingApproval
            )
                throw new InvalidOperationException(Resources.ActionUnsupported);
        }

        public static void VerifyCanApproveVariation(this WorkOrder wo)
        {
            if (wo.StatusCode != WorkStatusCode.PendingVariation) throw new NotSupportedException(Resources.ActionUnsupported);
        }

        public static void VerifyCanRejectVariation(this WorkOrder wo)
        {
            if (wo.StatusCode != WorkStatusCode.PendingVariation) throw new NotSupportedException(Resources.ActionUnsupported);
        }

        public static void VerifyCanApproveWorkOrder(this WorkOrder wo)
        {
            if (wo.StatusCode != Infrastructure.WorkStatusCode.PendingApproval) throw new NotSupportedException("Work order is not pending approval");
        }

        public static void VerifyCanRejectWorkOrder(this WorkOrder wo)
        {
            if (wo.StatusCode != Infrastructure.WorkStatusCode.PendingApproval) throw new NotSupportedException("Work order is not pending approval");
        }

        public static void VerifyCanAcknowldgeVariation(this WorkOrder wo)
        {
            if (
                wo.StatusCode != WorkStatusCode.VariationApproved &&
                wo.StatusCode != WorkStatusCode.VariationRejected
            )
                throw new NotSupportedException("Cannot Acknowledge");
        }

        public static void VerifyCanMoveToJobIncomplete(this WorkOrder wo)
        {
            if (wo.StatusCode == WorkStatusCode.PendingApproval) throw new NotSupportedException(Resources.ActionUnsupported);
        }
    }
}
