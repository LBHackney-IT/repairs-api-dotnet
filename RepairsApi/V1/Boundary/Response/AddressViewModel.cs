namespace RepairsApi.V1.Boundary.Response
{
    public class AddressViewModel
    { 
        /// <summary>
        /// Gets or Sets ShortAddress
        /// </summary>
        public string ShortAddress { get; set; }

        /// <summary>
        /// Gets or Sets PostalCode
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or Sets AddressLine
        /// </summary>
        public string AddressLine { get; set; }

        /// <summary>
        /// Gets or Sets StreetSuffix
        /// </summary>
        public string StreetSuffix { get; set; }
    }
}
