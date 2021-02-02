
namespace RepairsApi.V2.Domain
{
    public class Contractor
    {
        public string Reference { get; set; }
        public string Name { get; set; }
        public string ContractReference { get; internal set; }
        public double ContractCost { get; internal set; }
    }
}
