using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Identification
    {
        [Key] public int Id { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
    }


}

