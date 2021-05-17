using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class WorkOrderRejected : INotification
    {
        private WorkOrder _workOrder;

        public WorkOrderRejected(WorkOrder workOrder)
        {
            _workOrder = workOrder;
        }
    }
}
