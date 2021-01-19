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

    public class PropertyViewModel
    {
        /// <summary>
        /// Gets or Sets PropertyReference
        /// </summary>
        public string PropertyReference { get; set; }

        /// <summary>
        /// Gets or Sets Address
        /// </summary>
        public AddressViewModel Address { get; set; }

        /// <summary>
        /// Gets or Sets HierarchyType
        /// </summary>
        public HierarchyTypeViewModel HierarchyType { get; set; }

        /// <summary>
        /// Gets or Sets CanRaiseRepair
        /// </summary>
        public bool CanRaiseRepair { get; set; }
    }
}
