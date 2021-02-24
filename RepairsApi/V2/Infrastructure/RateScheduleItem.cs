using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class RateScheduleItem
    {
        [Key] public Guid Id { get; set; }
        public string M3NHFSORCode { get; set; }
        public string CustomCode { get; set; }
        public string CustomName { get; set; }
        public virtual Quantity Quantity { get; set; }
        public DateTime? DateCreated { get; set; }
        public double? CodeCost { get; set; }

        // extensions
        public bool Original { get; set; } = false;
        public double? OriginalQuantity { get; set; } = null;
    }
}
