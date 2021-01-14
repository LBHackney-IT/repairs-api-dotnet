using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class Identification
    {
        public string Type { get; set; }
        public string Number { get; set; }
    }


}

