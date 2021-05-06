using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class WorkOrderOpened : INotification
    {
        public WorkOrderOpened(WorkOrder workOrder)
        {
            WorkOrder = workOrder;
        }

        public WorkOrder WorkOrder { get; }
        public string TokenId { get; set; }
    }
}
