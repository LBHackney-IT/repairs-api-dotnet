using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V1.Infrastructure
{
    [Table("work_priority")]
    public class WorkPriority
    {
        [Key] [Column("id")] public Guid Id { get; set; }
        [Column("priority_description")] public string PriorityDescription { get; set; }
        [Column("comments")] public string Comments { get; set; }
        [Column("work_priority_code")] public WorkPriorityCode PriorityCode { get; set; }
        [Column("number_of_days")] public double NumberOfDays { get; set; }
        [Column("required_completion_datetime")] public DateTime RequiredCompletionDateTime { get; set; }
    }
}
