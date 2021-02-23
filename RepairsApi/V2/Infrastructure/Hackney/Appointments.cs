using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure.Hackney
{
    public class Appointment
    {
        public virtual AvailableAppointmentDay Day { get; set; }
        public int DayId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public int WorkOrderId { get; set; }
        public DateTime Date { get; set; }
    }

    public class AvailableAppointment
    {
        [Key] public int Id { get; set; }
        public virtual Contractor Contractor { get; set; }
        public string ContractorReference { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class AvailableAppointmentDay
    {
        [Key] public int Id { get; set; }
        public virtual AvailableAppointment AvailableAppointment { get; set; }
        public int AvailableAppointmentId { get; set; }
        public DayOfWeek Day { get; set; }
        public int AvailableCount { get; set; }

        public virtual List<Appointment> ExistingAppointments { get; set; }
    }
}
