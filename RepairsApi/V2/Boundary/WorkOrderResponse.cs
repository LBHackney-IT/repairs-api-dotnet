using RepairsApi.V2.Generated;
using System;

namespace RepairsApi.V2.Boundary
{
    public class WorkOrderResponse
    {
        public int Reference { get; set; }
        public DateTime? DateRaised { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Priority { get; set; }
        public string Property { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string PropertyReference { get; set; }
        public DateTime? Target { get; internal set; }
        public string RaisedBy { get; internal set; }
        public string CallerNumber { get; internal set; }
        public string CallerName { get; internal set; }
        public WorkPriorityCode? PriorityCode { get; internal set; }
        public string Status { get; internal set; }
        public string ContractorReference { get; set; }
        public string TradeCode { get; set; }
        public string TradeDescription { get; set; }
        public AppointmentResponse Appointment { get; set; }
        public bool HasVariation { get; set; }
    }
}
