using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure.Hackney
{
    public class ScheduleOfRates
    {
        [Key] public string CustomCode { get; set; }
        public string CustomName { get; set; }
        public double Cost { get; set; }
        public virtual SORPriority Priority { get; set; }
        public int PriorityId { get; set; }

        [Required]
        public SorCodeTrade Trade { get; set; }
        public string TradeCode { get; set; }

        public virtual List<SORContract> SorCodeMap { get; set; }
    }

    public class SORPriority
    {
        [Key] public int PriorityCode { get; set; }

        public string Description { get; set; }
    }

    public class SorCodeTrade
    {
        [Key] public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Contract
    {
        [Key] public string ContractReference { get; set; }
        public DateTime TerminationDate { get; set; }
        public DateTime EffectiveDate { get; set; }

        [Required]
        public virtual Contractor Contractor { get; set; }

        public virtual List<PropertyContract> PropertyMap { get; set; }
        public virtual List<SORContract> SorCodeMap { get; set; }
    }

    public class Contractor
    {
        [Key] public string Reference { get; set; }
        public string Name { get; set; }
    }

    public class SORContract
    {
        [Required]
        public virtual ScheduleOfRates SorCode { get; set; }
        public virtual string SorCodeCode { get; set; }
        public double Cost { get; set; }

        [Required]
        public virtual Contract Contract { get; set; }
        public string ContractReference { get; set; }
    }

    public class PropertyContract
    {
        public string PropRef { get; set; }
        public virtual Contract Contract { get; set; }
        public string ContractReference { get; set; }
    }
}
