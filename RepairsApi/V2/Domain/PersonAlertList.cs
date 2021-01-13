using System.Collections.Generic;

namespace RepairsApi.V2.Domain
{
    public class PersonAlertList
    {
        public IEnumerable<Alert> Alerts { get; set; }
    }
}
