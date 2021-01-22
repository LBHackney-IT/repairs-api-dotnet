using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Contact
    {
        [Key] public int Id { get; set; }
        public virtual Person Person { get; set; }
        public virtual List<PropertyAddress> Address { get; set; }
    }
}
