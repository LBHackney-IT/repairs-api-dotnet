using System;
using RepairsApi.V1.Infrastructure;
using RepairsApi.V2.Enums;

namespace RepairsApi.V1.Boundary.Response
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
    }
}
