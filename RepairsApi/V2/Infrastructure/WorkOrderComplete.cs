using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class WorkOrderComplete
    {
        [Key] public int Id { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual List<RateScheduleItem> BillOfMaterialItem { get; set; }
        public virtual List<WorkElement> CompletedWorkElements { get; set; }
        public virtual List<Operative> OperativesUsed { get; set; }
        public virtual List<JobStatusUpdate> JobStatusUpdates { get; set; }
        public virtual List<WorkOrder> FollowOnWorkOrder { get; set; }
    }
}

