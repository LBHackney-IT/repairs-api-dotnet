using System;

namespace RepairsApi.V2.Boundary.Response
{
    public class WorkOrderListItem
    {
        public int Reference { get; set; }
        public DateTime? DateRaised { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Priority { get; set; }
        public string Property { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string PropertyReference { get; set; }
    }
}
