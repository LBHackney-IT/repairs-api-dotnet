using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V2.Infrastructure
{
    public class Operative : IArchivable
    {
        [Key] public int Id { get; set; }
        public string PayrollNumber { get; set; }
        public bool IsArchived { get; set; }
        public virtual Person Person { get; set; }
        public virtual List<Trade> Trade { get; set; }
        public virtual List<WorkElement> WorkElement { get; set; }
    }
}

