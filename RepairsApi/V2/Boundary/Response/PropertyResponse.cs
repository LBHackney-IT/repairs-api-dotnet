namespace RepairsApi.V2.Boundary.Response
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
        public AlertsViewModel Alerts { get; set; }

        /// <summary>
        /// Gets or Sets Tenure Information
        /// </summary>
        public TenureViewModel Tenure { get; set; }
    }
}
