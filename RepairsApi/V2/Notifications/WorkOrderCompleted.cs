using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class WorkOrderCompleted : INotification
    {
        public WorkOrderCompleted(WorkOrder workOrder, JobStatusUpdate update)
        {
            WorkOrder = workOrder;
            Update = update;
        }

        public WorkOrder WorkOrder { get; }
        public JobStatusUpdate Update { get; }
    }
}
