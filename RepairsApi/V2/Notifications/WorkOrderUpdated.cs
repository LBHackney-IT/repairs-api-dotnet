using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class WorkOrderUpdated : INotification
    {
        public WorkOrder WorkOrder { get; }

        public WorkOrderUpdated(WorkOrder workOrder) => WorkOrder = workOrder;
    }
}
