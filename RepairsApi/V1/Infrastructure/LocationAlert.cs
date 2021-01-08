using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class LocationAlert
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("comments")] public string Comments { get; set; }
        [Column("type")] public LocationAlertTypeCode Type { get; set; }
    }
}
