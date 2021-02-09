using System;
using System.Collections.Generic;

namespace RepairsApi.V2.Controllers
{
    public class AppointmentDayViewModel
    {
        public DateTime Date { get; set; }
        public IEnumerable<AppointmentSlot> Slots { get; set; }
    }

    public class AppointmentSlot
    {
        public int Reference { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
