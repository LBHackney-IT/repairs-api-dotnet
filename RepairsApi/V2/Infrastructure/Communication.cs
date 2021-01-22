using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class Communication
    {
        public virtual CommunicationChannel Channel { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool NotAvailable { get; set; }
    }
}

