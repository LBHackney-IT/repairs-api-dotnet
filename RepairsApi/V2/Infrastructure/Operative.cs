using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Operative
    {
        [Key] public int Id { get; set; }
        public virtual Person Person { get; set; }
        public virtual List<Trade> Trade { get; set; }
        public virtual List<WorkElement> WorkElement { get; set; }
    }
}
