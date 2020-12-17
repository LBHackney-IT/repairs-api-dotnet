namespace RepairsApi.V1.Boundary.Response
{
    public class TenureViewModel
    {
        /// <summary>
        /// Gets or Sets Typecode
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Gets or Sets TypeDescription
        /// </summary>
        public string TypeDescription { get; set; }

        /// <summary>
        /// Gets or Sets CanRaiseRepair
        /// </summary>
        public bool CanRaiseRepair { get; set; }
    }
}
