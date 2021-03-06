using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class HighCostWorkOrderCreated : INotification
    {
        public WorkOrder WorkOrder { get; }

        public HighCostWorkOrderCreated(WorkOrder workOrder)
        {
            WorkOrder = workOrder;
        }
    }
}
