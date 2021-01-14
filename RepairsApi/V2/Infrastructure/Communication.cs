using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Communication
    {
        [Key] public int Id { get; set; }
        public virtual CommunicationChannel Channel { get; set; }
        public string Value { get; set; }
    }


}

