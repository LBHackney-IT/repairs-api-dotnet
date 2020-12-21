using System.Collections.Generic;

namespace RepairsApi.V1.Domain
{
    public class PersonAlertList
    {
        public IEnumerable<Alert> Alerts { get; set; }
    }
}
