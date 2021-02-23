using System.Collections.Generic;

namespace RepairsApi.V2.Gateways.Models
{
    public class ResidentContactInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<PhoneNumberType> PhoneNumbers { get; set; }
    }

    public class PhoneNumberType
    {
        public string LastModified { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneType { get; set; }
    }
}
