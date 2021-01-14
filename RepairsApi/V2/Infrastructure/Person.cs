using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Person
    {
        [Key] public int Id { get; set; }
        public virtual PersonName Name { get; set; }
        public virtual Identification Identification { get; set; }
    }
}

