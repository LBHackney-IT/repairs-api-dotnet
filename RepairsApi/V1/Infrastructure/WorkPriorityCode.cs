using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V1.Infrastructure
{
    [Table("work_priority_code")]
    public class WorkPriorityCode
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; }
    }
}
