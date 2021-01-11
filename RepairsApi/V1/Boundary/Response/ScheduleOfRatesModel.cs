using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V1.Domain;

namespace RepairsApi.V1.Boundary.Response
{
    public class ScheduleOfRatesModel
    {
        public string CustomCode { get; set; }
        public string CustomName { get; set; }

        public SORPriority Priority { get; set; }

        public Contractor SORContractor { get; set; }


    }
}
