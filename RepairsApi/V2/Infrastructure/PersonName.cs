using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class PersonName
    {
        [Key] public int Id { get; set; }
        public string Full { get; set; }
    }


}

