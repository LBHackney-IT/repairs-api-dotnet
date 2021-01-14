
using System;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Appointment
    {
        [Key] public int Id { get; set; }
        public DateTime? Date { get; set; }
        public virtual AppointmentTimeOfDay TimeOfDay { get; set; }
    }
}

