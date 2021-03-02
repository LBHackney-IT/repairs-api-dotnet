using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Infrastructure
{
    public class AppointmentDetails
    {
        public DateTime Date { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
