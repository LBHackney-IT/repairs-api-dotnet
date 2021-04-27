
namespace RepairsApi.V2.Domain
{
    public class Contractor
    {
        public string ContractorReference { get; set; }
        public string ContractorName { get; set; }
        public bool UseExternalScheduleManager { get; set; }
    }
}
