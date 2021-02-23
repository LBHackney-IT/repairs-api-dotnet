namespace RepairsApi.V2.Domain
{
    public class TenureInformation
    {
        public string TenancyAgreementReference { get; set; }
        public string HouseholdReference { get; set; }
        public string TypeDescription { get; set; }
        public string TypeCode { get; set; }
        public bool CanRaiseRepair { get; set; }
    }
}
