
namespace RepairsApi.V2.Domain
{
    public class Contractor
    {
        public Contractor(string contractorReference, string contractorName)
        {
            this.ContractorReference = contractorReference;
            this.ContractorName = contractorName;
        }

        public Contractor() {}

        public string ContractorReference { get; set; }
        public string ContractorName { get; set; }
        public bool UseExternalScheduleManager { get; set; }
    }
}
