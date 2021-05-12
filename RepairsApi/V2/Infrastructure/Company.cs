using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Company
    {
        [StringLength(10)]
        public string CoCode { get; set; }
        [StringLength(10)]
        public string GroupCode { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(10)]
        public string CloneOf { get; set; }
        public char CGroup { get; set; }
        [StringLength(200)]
        public string CompAvail { get; set; }
        [StringLength(200)]
        public string CompDisplay { get; set; }
        public bool CoEnabled { get; set; }
        public int CompanySid { get; set; }
    }
}
