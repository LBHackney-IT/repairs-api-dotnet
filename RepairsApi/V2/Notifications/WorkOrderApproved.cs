using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class WorkOrderApproved : INotification
    {
        private WorkOrder _workOrder;

        public WorkOrderApproved(WorkOrder workOrder)
        {
            _workOrder = workOrder;
        }
    }
}
