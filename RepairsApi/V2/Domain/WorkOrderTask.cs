using System;

namespace RepairsApi.V2.Domain
{
    public class WorkOrderTask
    {
        public double Quantity { get; internal set; }
        public string Code { get; internal set; }
        public double? Cost { get; internal set; }
        public DateTime? DateAdded { get; internal set; }
        public string Description { get; internal set; }
        public string Status { get; internal set; }
        public bool Original { get; set; }
        public Guid Id { get; set; }
        public double? OriginalQuantity { get; set; }
    }
}
