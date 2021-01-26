using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class ScheduleOfRates
    {
        [Key] public string CustomCode { get; set; }
        public string CustomName { get; set; }

        public virtual SORPriority Priority { get; set; }
        public int PriorityId { get; set; }

        public virtual SORTrade Trade { get; set; }
        public string TradeCode { get; set; }
        public virtual SORContractor Contractor { get; set; }
        public string SORContractorRef { get; set; }

    }

    public class SORPriority
    {
        [Key] public int PriorityCode { get; set; }

        public string Description { get; set; }
    }

    public class SORTrade
    {
        [Key] public string TradeCode { get; set; }

        public string Description { get; set; }
    }

    public class SORContractor
    {
        [Key] public string SORContractorRef { get; set; }
        public string Name { get; set; }
    }
}
