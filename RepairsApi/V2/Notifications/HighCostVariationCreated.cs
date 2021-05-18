using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class HighCostVariationCreated : INotification
    {
        public WorkOrder WorkOrder { get; }

        public HighCostVariationCreated(WorkOrder workOrder)
        {
            WorkOrder = workOrder;
        }
    }
}
