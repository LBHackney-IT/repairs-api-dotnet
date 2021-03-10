using System.Collections.Generic;

namespace RepairsApi.V2.Domain
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Groups { get; set; }

        public double? VarySpendLimit { get; set; }
        public double? RaiseSpendLimit { get; set; }
    }
}
