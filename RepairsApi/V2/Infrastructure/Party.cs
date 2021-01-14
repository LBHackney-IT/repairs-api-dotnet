using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Party
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}

