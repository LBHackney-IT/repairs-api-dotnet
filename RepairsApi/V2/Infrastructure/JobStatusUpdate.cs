using RepairsApi.V2.Generated;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class JobStatusUpdate
    {
        [Key] public int Id { get; set; }
        public virtual List<WorkElement> RelatedWorkElement { get; set; }
        public DateTime? EventTime { get; set; }
        public JobStatusUpdateTypeCode? TypeCode { get; set; }
        public string OtherType { get; set; }
        public virtual List<Person> OperativesAssigned { get; set; }
        public virtual Appointment RefinedAppointmentWindow { get; set; }
        public virtual CustomerSatisfaction CustomerFeedback { get; set; }
        public virtual Communication CustomerCommunicationChannelAttempted { get; set; }
        public string Comments { get; set; }
        public virtual WorkElement MoreSpecificSORCode { get; set; }
        public virtual AdditionalWork AdditionalWork { get; set; }
        public virtual WorkOrder RelatedWorkOrder { get; set; }

        // extensions
        public string Author { get; set; }
    }
}
