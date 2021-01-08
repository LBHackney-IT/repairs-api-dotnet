using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V1.Infrastructure
{
    [Owned]
    public class KeySafe
    {
        public string Location { get; set; }
        public string Code { get; set; }
    }
}
