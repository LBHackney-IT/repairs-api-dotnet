using Microsoft.EntityFrameworkCore;

namespace RepairsApi.V2.Infrastructure
{
    [Owned]
    public class PersonName
    {
        public string Full { get; set; }
        public string Given { get; set; }
        public string Family { get; set; }
        public string FamilyPrefix { get; set; }
        public string Initials { get; set; }
        public string Title { get; set; }
        public string Middle { get; set; }
    }
}
