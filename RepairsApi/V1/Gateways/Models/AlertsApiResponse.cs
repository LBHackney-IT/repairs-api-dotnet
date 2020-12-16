using System.Collections.Generic;

namespace RepairsApi.V1.Gateways.Models
{
    public class AlertsApiResponse
    {
        public string PropertyReference { get; set; }
        public List<AlertApiAlertViewModel> Alerts { get; set; }
    }

    public class AlertApiAlertViewModel
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string AlertCode { get; set; }

        public string Description { get; set; }
    }
}
