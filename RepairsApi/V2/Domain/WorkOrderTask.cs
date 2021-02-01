using System;

namespace RepairsApi.V2.Domain
{
    public class WorkOrderTask
    {
        public double Quantity { get; internal set; }
        public string Id { get; internal set; }
        public double? Cost { get; internal set; }
        public DateTime? DateAdded { get; internal set; }
        public string Description { get; internal set; }
        public string Status { get; internal set; }
    }
}