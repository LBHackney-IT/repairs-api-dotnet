using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Infrastructure.Hackney;
using System;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class WorkPriority
    {
        public int? PriorityCode { get; set; }
        public virtual SORPriority Priority { get; set; }
        public DateTime? RequiredCompletionDateTime { get; set; }
        public string PriorityDescription { get; set; }
        public string Comments { get; set; }
        public double? NumberOfDays { get; set; }
    }
}
