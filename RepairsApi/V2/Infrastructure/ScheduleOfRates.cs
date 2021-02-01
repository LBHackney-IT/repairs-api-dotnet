using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class ScheduleOfRates
    {
        [Key] public string CustomCode { get; set; }
        public string CustomName { get; set; }
        public double Cost { get; set; }
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

        public virtual List<SORContractor> Contractors { get; set; }
    }

    public class SORContractor
    {
        [Key] public string SORContractorRef { get; set; }
        public string Name { get; set; }

        public virtual List<SORContract> Contracts { get; set; }
    }

    public class SORContract
    {
        [Key] public string SORContractRef { get; set; }

        public int PropertyConstraintMethodId { get; set; }
        public string PropertyConstraintParameters { get; set; }

        public virtual SORContractor Contractor { get; set; }
        public virtual List<SORTrade> Contractors { get; set; }
    }
}
