using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CostSubjectCode = RepairsApi.V2.Generated.CostSubjectCode;

namespace RepairsApi.V2.Infrastructure
{
    public class WorkElement
    {
        [Key] public Guid Id { get; set; }
        public CostSubjectCode? ServiceChargeSubject { get; set; }
        public bool? ContainsCapitalWork { get; set; }

        public virtual List<RateScheduleItem> RateScheduleItem { get; set; }
        public virtual List<Trade> Trade { get; set; }
    }
}
