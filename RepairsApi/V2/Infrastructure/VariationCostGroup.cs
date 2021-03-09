using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V2.Infrastructure
{
    public class VariationCostGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double VariationCostLimit { get; set; }

        public string CostGroupName { get; set; }
    }
}
