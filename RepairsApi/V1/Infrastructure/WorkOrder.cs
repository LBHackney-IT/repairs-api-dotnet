using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V1.Infrastructure
{
    [Table("work_order")]
    public class WorkOrder
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("description_of_work")] public string DescriptionOfWork { get; set; }
        [Column("priority")] public virtual WorkPriority Priority { get; set; }
        public virtual List<SitePropertyUnit> SitePropertyUnits { get; set; }
        public virtual List<WorkElement> WorkElements { get; set; }
        [Column("work_class")] public virtual WorkClass WorkClass { get; set; }
    }

}
