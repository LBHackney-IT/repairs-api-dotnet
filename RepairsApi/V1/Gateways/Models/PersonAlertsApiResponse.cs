using System.Collections.Generic;

namespace RepairsApi.V1.Gateways.Models
{
    public class PersonAlertsApiResponse
    {
        public string TenancyAgreementReference { get; set; }
        public List<AlertApiAlertViewModel> Alerts { get; set; }
    }

    public class ListPersonAlertsApiResponse
    {
        public List<PersonAlertsApiResponse> Contacts { get; set; }
    }
}
