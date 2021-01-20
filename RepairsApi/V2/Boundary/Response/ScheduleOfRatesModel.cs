using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Domain;

namespace RepairsApi.V2.Boundary.Response
{
    public class ScheduleOfRatesModel
    {
        public string CustomCode { get; set; }
        public string CustomName { get; set; }

        public SORPriority Priority { get; set; }

        public Contractor SORContractor { get; set; }
    }
}
