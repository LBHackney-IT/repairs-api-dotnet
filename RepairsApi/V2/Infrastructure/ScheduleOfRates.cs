using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Domain;

namespace RepairsApi.V2.Infrastructure
{
    public class ScheduleOfRates
    {
        [Key] public string CustomCode { get; set; }
        public string CustomName { get; set; }

        public virtual SORPriority Priority { get; set; }
        public int PriorityId { get; set; }

        public string SORContractorRef { get; set; }
    }

    public class SORPriority
    {
        [Key] public int PriorityCode { get; set; }

        public string Description { get; set; }
    }
}
