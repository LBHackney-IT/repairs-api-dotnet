using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class RateScheduleItem
    {
        [Key] public Guid Id { get; set; }
        public string M3NHFSORCode { get; set; }
        public string CustomCode { get; set; }
        public string CustomName { get; set; }
        public virtual Quantity Quantity { get; set; }
    }
}
