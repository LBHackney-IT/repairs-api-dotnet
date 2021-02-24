using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class SecurityGroup
    {
        [Key] public string GroupName { get; set; }
        public string UserType { get; set; }
        public string ContractorReference { get; set; }
        public double? RaiseLimit { get; set; }
        public double? VaryLimit { get; set; }
    }
}
