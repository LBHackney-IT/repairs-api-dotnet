using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Categorization
    {
        [Key] public int Id { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string VersionUsed { get; set; }
        public string SubCategory { get; set; }
    }
}

