using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RepairsApi.V1.Generated;

namespace RepairsApi.V1.Infrastructure
{
    public class AlertRegardingPerson
    {
        [Key] public int Id { get; set; }
        public string Comments { get; set; }
        public PersonAlertTypeCode Type { get; set; }
    }

}
