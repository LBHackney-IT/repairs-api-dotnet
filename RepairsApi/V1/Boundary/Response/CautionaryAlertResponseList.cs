using System.Collections.Generic;

namespace RepairsApi.V1.Boundary.Response
{
    public class CautionaryAlertResponseList
    {
        /// <summary>
        /// Gets or Sets PropertyReference
        /// </summary>
        public string PropertyReference { get; set; }

        /// <summary>
        /// Gets or Sets Alerts
        /// </summary>
        public List<CautionaryAlertViewModel> Alerts { get; set; }
    }
}
