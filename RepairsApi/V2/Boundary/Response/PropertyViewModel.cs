namespace RepairsApi.V2.Boundary.Response
{
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
    }
}
