using System.Collections.Generic;

namespace RepairsApi.V2.Domain
{
    public class PropertyAlertList
    {
        public string PropertyReference { get; set; }
        public IEnumerable<Alert> Alerts { get; set; }
    }
}
