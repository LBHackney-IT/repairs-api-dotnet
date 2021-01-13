using RepairsApi.V1.Generated;
using System;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V1.Infrastructure
{
    public class WorkPriority
    {
        [Key] public Guid Id { get; set; }
        public WorkPriorityCode? PriorityCode { get; set; }
        public DateTime? RequiredCompletionDateTime { get; set; }
        public string PriorityDescription { get; set; }
        public string Comments { get; set; }
        public double? NumberOfDays { get; set; }
    }
}
