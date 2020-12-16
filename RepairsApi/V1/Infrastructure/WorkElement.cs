using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V1.Infrastructure
{
    [Table("work_element")]
    public class WorkElement
    {
        [Key] [Column("id")] public Guid Id { get; set; }
        [Column("trade ")] public string trade { get; set; }
        [Column("service_charge_subject")] public string ServiceChargeSubject { get; set; }
        [Column("contains_capital_work")] public bool ContainsCapitalWork { get; set; }

        List<RateScheduleItem> RateScheduleItem { get; set; }
    }
}
