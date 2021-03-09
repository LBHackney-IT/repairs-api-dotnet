using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairsApi.V2.Infrastructure
{
    public class RepairsHubUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string EmailAddress { get; set; }

        [ForeignKey("VariationCostGroup")]
        public int VariationGroupId { get; set; }
    }
}
