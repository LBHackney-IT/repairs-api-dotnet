using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class PersonAlert
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("comments")] public string Comments { get; set; }
        public PersonAlertTypeCode Type { get; set; }
    }

}
