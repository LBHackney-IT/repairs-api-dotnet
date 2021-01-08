using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class WorkOrder
    {
        [Key] public int Id { get; set; }
        public string DescriptionOfWork { get; set; }
        public double EstimatedLaborHours { get; set; }
        public WorkType WorkType { get; set; }
        public string ParkingArrangements { get; set; }
        public string LocationOfRepair { get; set; }
        public DateTime DateReported { get; set; }
        public virtual WorkClass WorkClass { get; set; }

        public virtual WorkPriority WorkPriority { get; set; }
        public virtual Site Site { get; set; }
        public virtual WorkOrderAccessInformation AccessInformation { get; set; }
        public virtual List<WorkElement> WorkElements { get; set; }
        public virtual List<AlertRegardingPerson> PersonAlert { get; set; }
        public virtual List<AlertRegardingLocation> LocationAlert { get; set; }
    }

    public class WorkOrderAccessInformation
    {
        [Key] public int Id { get; set; }
        public string Description { get; set; }
        public virtual KeySafe Keysafe { get; set; }
    }

}
