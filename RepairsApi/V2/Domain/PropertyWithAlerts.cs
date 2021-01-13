using System.Collections.Generic;

namespace RepairsApi.V2.Domain
{
    public class PropertyWithAlerts
    {
        public PropertyModel PropertyModel { get; set; }
        public IEnumerable<Alert> LocationAlerts { get; set; }
        public IEnumerable<Alert> PersonAlerts { get; set; }
        public TenureInformation Tenure { get; set; }
    }
}
