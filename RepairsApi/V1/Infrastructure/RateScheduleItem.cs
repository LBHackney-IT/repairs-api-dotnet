using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V1.Infrastructure
{
    [Table("rate_schedule_item")]
    public class RateScheduleItem
    {
        [Key] [Column("id")] public Guid Id { get; set; }

        [Column("custom_code")] public string CustomCode { get; set; }

        [Column("custom_name")] public string CustomName { get; set; }
        public virtual Quantity Quantity { get; set; }

        [Column("work_element_id")] public Guid WorkElementId { get; set; }
        public virtual WorkElement WorkElement { get; set; }
    }
}
