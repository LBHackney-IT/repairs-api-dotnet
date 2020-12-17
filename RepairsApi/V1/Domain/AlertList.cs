using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V1.Domain
{
    public class AlertList
    {
        public PropertyAlertList PropertyAlerts { get; set; }
        public PersonAlertList PersonAlerts { get; set; }
    }
}
