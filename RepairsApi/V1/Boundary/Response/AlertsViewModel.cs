using System.Collections.Generic;

namespace RepairsApi.V1.Boundary.Response
{
    public class AlertsViewModel
    {
        public List<CautionaryAlertViewModel> LocationAlert { get; set; }
        public List<CautionaryAlertViewModel> PersonAlert { get; set; }
    }
}
