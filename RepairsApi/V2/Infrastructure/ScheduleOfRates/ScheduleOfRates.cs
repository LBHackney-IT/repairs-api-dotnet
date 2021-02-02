using System;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure.ScheduleOfRates
{
    public class ScheduleOfRates
    {
        [Key] public string CustomCode { get; set; }
        public string CustomName { get; set; }
        public double Cost { get; set; }
        public virtual SORPriority Priority { get; set; }
        public int PriorityId { get; set; }

        public Trade Trade { get; set; }
    }

    public class SORPriority
    {
        [Key] public int PriorityCode { get; set; }

        public string Description { get; set; }
    }

    public class Trade
    {
        [Key]public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Contract
    {
        [Key]public string ContractReference { get; set; }
        public DateTime TerminationDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public virtual Contractor Contractor { get; set; }
    }

    public class Contractor
    {
        [Key]public string Code { get; set; }
        public string Name { get; set; }
    }

    public class SORContract
    {
        public virtual ScheduleOfRates SorCode { get; set; }
        public virtual Contract Contract { get; set; }
        public double Cost { get; set; }
    }

    public class PropertyContract
    {
        public string PropRef { get; set; }
        public virtual Contract Contract { get; set; }
    }
}
