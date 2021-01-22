using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Person
    {
        [Key] public int Id { get; set; }
        public virtual PersonName Name { get; set; }
        public virtual List<PersonName> AliasNames { get; set; }
        public virtual List<Communication> Communication { get; set; }
    }
}

