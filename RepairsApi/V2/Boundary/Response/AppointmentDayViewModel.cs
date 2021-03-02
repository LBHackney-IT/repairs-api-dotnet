using System;
using System.Collections.Generic;

namespace RepairsApi.V2.Boundary.Response
{
    public class AppointmentDayViewModel
    {
        public string Date { get; set; }
        public IEnumerable<AppointmentSlot> Slots { get; set; }
    }

    public class AppointmentSlot
    {
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
