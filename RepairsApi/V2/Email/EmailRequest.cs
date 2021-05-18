using System.Collections.Generic;

namespace RepairsApi.V2.Email
{
    public class EmailRequest : Dictionary<string, object>
    {
        public string Address { get; set; }

        public EmailRequest(string address) => Address = address;

        public void Set(string key, object value) => this[key] = value;
    }
}
