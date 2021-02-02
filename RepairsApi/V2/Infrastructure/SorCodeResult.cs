using System.Collections.Generic;

namespace RepairsApi.V2.Infrastructure
{
    public class SorCodeResult
    {
        public IEnumerable<SorCodeContractResult> Contracts { get; internal set; }
        public string PriorityDescription { get; set; }
        public int PriorityCode { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }

    public class SorCodeContractResult
    {
        public double ContractCost { get; internal set; }
        public string ContractorName { get; internal set; }
        public string ContractReference { get; internal set; }
        public string ContractorCode { get; internal set; }
    }
}
