using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Company
    {
        [Key]
        [StringLength(10)]
        public string CoCode { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(200)]
        public string CompAvail { get; set; }
    }
}
