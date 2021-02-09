using System;
using System.Collections.Generic;

namespace RepairsApi.V2.Controllers
{
    public class AppointmentDayViewModel
    {
        public DateTime Date { get; set; }
        public List<AppointmentSlot> Slots { get; set; }
    }

    public class AppointmentSlot
    {
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
