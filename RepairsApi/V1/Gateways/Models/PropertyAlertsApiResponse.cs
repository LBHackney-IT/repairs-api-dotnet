using System.Collections.Generic;

namespace RepairsApi.V1.Gateways.Models
{
    public class PropertyAlertsApiResponse
    {
        public string PropertyReference { get; set; }
        public List<AlertApiAlertViewModel> Alerts { get; set; }
    }
}
