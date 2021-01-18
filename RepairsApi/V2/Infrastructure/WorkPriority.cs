using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Generated;
using System;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class WorkPriority
    {
        public WorkPriorityCode? PriorityCode { get; set; }
        public DateTime? RequiredCompletionDateTime { get; set; }
        public string PriorityDescription { get; set; }
        public string Comments { get; set; }
        public double? NumberOfDays { get; set; }
    }
}
