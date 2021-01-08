using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V1.Infrastructure
{
    public class WorkPriority
    {
        [Key] public Guid Id { get; set; }
        public virtual WorkPriorityCode PriorityCode { get; set; }
        public DateTime RequiredCompletionDateTime { get; set; }
    }
}
