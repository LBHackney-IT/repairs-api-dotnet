using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.Infrastructure
{
    public class WorkOrder
    {
        [Key] public int Id { get; set; }
        public string DescriptionOfWork { get; set; }
        public double? EstimatedLaborHours { get; set; }
        public WorkType? WorkType { get; set; }
        public string ParkingArrangements { get; set; }
        public string LocationOfRepair { get; set; }
        public DateTime? DateRaised { get; set; }
        public DateTime? DateReported { get; set; }
        public virtual WorkClass WorkClass { get; set; }
        public virtual Organization InstructedBy { get; set; }
        public virtual Party AssignedToPrimary { get; set; }

        public virtual Party Customer { get; set; }
        public virtual WorkPriority WorkPriority { get; set; }
        public virtual Site Site { get; set; }
        public virtual WorkOrderAccessInformation AccessInformation { get; set; }
        public virtual List<WorkElement> WorkElements { get; set; }
        public virtual List<AlertRegardingPerson> PersonAlert { get; set; }
        public virtual List<AlertRegardingLocation> LocationAlert { get; set; }
        public virtual WorkOrderComplete WorkOrderComplete { get; set; }
    }

    [Owned]
    public class WorkOrderAccessInformation
    {
        public string Description { get; set; }
        public virtual KeySafe Keysafe { get; set; }
    }
}
