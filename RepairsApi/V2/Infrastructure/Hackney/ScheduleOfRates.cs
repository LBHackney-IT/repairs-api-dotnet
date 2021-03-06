using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure.Hackney
{
    public class ScheduleOfRates
    {
        [Key] public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public double? Cost { get; set; }
        public virtual SORPriority Priority { get; set; }
        public int? PriorityId { get; set; }

        [Required]
        public virtual SorCodeTrade Trade { get; set; }
        public string TradeCode { get; set; }

        [Required]
        public virtual List<SORContract> SorCodeMap { get; set; }

        [Required]
        public int StandardMinuteValue { get; set; } = 0;
        public bool Enabled { get; set; }
    }

    public class SORPriority
    {
        [Key] public int PriorityCode { get; set; }

        public string Description { get; set; }

        public char PriorityCharacter { get; set; }

        public int? DaysToComplete { get; set; }

        public bool Enabled { get; set; }
    }

    public class SorCodeTrade
    {
        public SorCodeTrade(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }

        public SorCodeTrade() { }

        [Key] public string Code { get; set; }
        public string Name { get; set; }

        public virtual List<Operative> Operatives { get; set; }
    }

    public class Contract
    {
        [Key] public string ContractReference { get; set; }
        public DateTime TerminationDate { get; set; }
        public DateTime EffectiveDate { get; set; }

        [Required]
        public virtual Contractor Contractor { get; set; }
        public string ContractorReference { get; set; }

        public virtual List<PropertyContract> PropertyMap { get; set; }
        public virtual List<SORContract> SorCodeMap { get; set; }
    }

    public class Contractor
    {
        [Key] public string Reference { get; set; }
        public string Name { get; set; }
        public bool UseExternalScheduleManager { get; set; }
        public bool CanAssignOperative { get; set; }
        public virtual List<Contract> Contracts { get; set; }
        public string ContractManagerEmail { get; set; }
    }

    public class SORContract
    {
        [Required]
        public virtual ScheduleOfRates SorCode { get; set; }
        public string SorCodeCode { get; set; }
        public double? Cost { get; set; }

        [Required]
        public virtual Contract Contract { get; set; }
        public string ContractReference { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SORContract contract &&
                   SorCodeCode == contract.SorCodeCode &&
                   ContractReference == contract.ContractReference;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SorCodeCode, ContractReference);
        }
    }

    public class PropertyContract
    {
        [Required]
        public string PropRef { get; set; }

        [Required]
        public virtual Contract Contract { get; set; }
        public string ContractReference { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PropertyContract contract &&
                   PropRef == contract.PropRef &&
                   ContractReference == contract.ContractReference;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PropRef, ContractReference);
        }
    }
}
