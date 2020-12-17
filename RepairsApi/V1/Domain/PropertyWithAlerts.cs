using System.Collections.Generic;

namespace RepairsApi.V1.Domain
{
    public class PropertyWithAlerts
    {
        public PropertyModel PropertyModel { get; set; }
        public IEnumerable<PropertyAlert> Alerts { get; set; }
    }
}
