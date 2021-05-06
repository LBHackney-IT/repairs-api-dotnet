using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class WorkOrderCompleted : INotification
    {
        public WorkOrderCompleted(WorkOrder workOrder, JobStatusUpdates update)
        {
            WorkOrder = workOrder;
            Update = update;
        }

        public WorkOrder WorkOrder { get; }
        public JobStatusUpdates Update { get; }
    }
}
