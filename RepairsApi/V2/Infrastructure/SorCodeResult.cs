using System.Collections.Generic;

namespace RepairsApi.V2.Infrastructure
{
    public class SorCodeResult
    {
        public IEnumerable<SorCodeContractResult> Contracts { get; set; }
        public string PriorityDescription { get; set; }
        public int PriorityCode { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }

    public class SorCodeContractResult
    {
        public double ContractCost { get; set; }
        public string ContractorName { get; set; }
        public string ContractReference { get; set; }
        public string ContractorCode { get; set; }
    }
}
