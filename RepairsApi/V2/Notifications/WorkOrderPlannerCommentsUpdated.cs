using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class WorkOrderPlannerCommentsUpdated : INotification
    {
        public WorkOrderPlannerCommentsUpdated(WorkOrder workOrder)
        {
            WorkOrder = workOrder;
        }

        public WorkOrder WorkOrder { get; }
        public string TokenId { get; set; }
    }
}
