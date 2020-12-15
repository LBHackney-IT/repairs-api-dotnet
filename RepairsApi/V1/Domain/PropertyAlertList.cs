using System.Collections.Generic;

namespace RepairsApi.V1.Domain
{
    public class PropertyAlertList
    {
        public string PropertyReference { get; set; }
        public IEnumerable<PropertyAlert> Alerts { get; set; }
    }
}
