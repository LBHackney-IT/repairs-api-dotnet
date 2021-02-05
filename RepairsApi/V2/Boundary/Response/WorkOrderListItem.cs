using System;

namespace RepairsApi.V2.Boundary.Response
{
    public static class WorkOrderStatusResp
    {
        public const string InProgress = "In Progress";
        public const string Complete = "Work Complete";
        public const string Cancelled = "Work Cancelled";
        public const string Unknown = "Unknown";
    }

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
        public string TradeCode { get; set; }
        public string Status { get; set; }
    }
}
