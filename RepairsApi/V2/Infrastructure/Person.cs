using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.Infrastructure
{
    public class Person
    {
        [Key] public int Id { get; set; }
        public virtual PersonName Name { get; set; }
        public virtual Identification Identification { get; set; }
        public virtual List<PersonName> AliasNames { get; set; }
        public virtual List<Communication> Communication { get; set; }

        public string GetPhoneNumber()
        {
            // get first audio communication
            return Communication.FirstOrDefault(c => c.Channel.Medium == CommunicationMediumCode._20)?.Value;
        }
    }
}

