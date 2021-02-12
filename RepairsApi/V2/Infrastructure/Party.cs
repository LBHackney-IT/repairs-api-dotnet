using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Party
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Person Person { get; set; }
        public string ContractorReference { get; set; }
    }
}

