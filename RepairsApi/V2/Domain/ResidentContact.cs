using System.Collections.Generic;

namespace RepairsApi.V2.Domain
{
    public class ResidentContact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }
}
