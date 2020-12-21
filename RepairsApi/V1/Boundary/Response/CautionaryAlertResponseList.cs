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
        /// Gets or Sets Location Alerts
        /// </summary>
        public List<CautionaryAlertViewModel> LocationAlert { get; set; }

        /// <summary>
        /// Gets or Sets Person Alerts
        /// </summary>
        public List<CautionaryAlertViewModel> PersonAlert { get; set; }
    }
}
