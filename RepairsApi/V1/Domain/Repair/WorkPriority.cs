using System;

namespace RepairsApi.V1.Domain.Repair
{
    public class WorkPriority
    {
        public string PriorityCode { get; set; }

        public DateTime RequiredCompletionDateTime { get; set; }
    }
}