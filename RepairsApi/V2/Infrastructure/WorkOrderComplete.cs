using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V2.Infrastructure
{
    public class WorkOrderComplete
    {
        [Key] [ForeignKey("WorkOrder")] public int Id { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual List<RateScheduleItem> BillOfMaterialItem { get; set; }
        public virtual List<WorkElement> CompletedWorkElements { get; set; }
        public virtual List<JobStatusUpdate> JobStatusUpdates { get; set; }
        public virtual List<WorkOrder> FollowOnWorkOrder { get; set; }
        public DateTime? ClosedTime { get; set; }
    }
}
