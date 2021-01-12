namespace RepairsApi.V1.Domain
{
    public class Address
    {
        public string AddressLine { get; internal set; }
        public string PostalCode { get; internal set; }
        public string ShortAddress { get; internal set; }
        public string StreetSuffix { get; internal set; }
    }
}
