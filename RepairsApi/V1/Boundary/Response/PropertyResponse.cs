using System.Collections.Generic;

namespace RepairsApi.V1.Boundary.Response
{
    public class PropertyResponse
    { 
        /// <summary>
        /// Gets or Sets Property
        /// </summary>
        public PropertyViewModel Property { get; set; }

        /// <summary>
        /// Gets or Sets CautionaryAlerts
        /// </summary>
        public List<CautionaryAlertViewModel> CautionaryAlerts { get; set; }
    }
}
