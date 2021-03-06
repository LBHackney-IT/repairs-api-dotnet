using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V2.Infrastructure
{
    public class Organization
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string DoingBusinessAsName { get; set; }
    }
}
