using System.Collections.Generic;

namespace RepairsApi.V2.Domain
{
    public class User
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Groups { get; set; }

        public double? VaryLimit { get; set; }
        public double? RaiseLimit { get; set; }
    }
}
