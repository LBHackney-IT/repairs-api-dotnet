using System;

namespace RepairsApi.V2.Boundary.Response
{
    public class WorkOrderItemViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? DateAdded { get; set; }
        public double Quantity { get; set; }
        public double? Cost { get; set; }
        public string Status { get; set; }
        public bool Original { get; set; }
        public double? OriginalQuantity { get; set; }
    }
}
