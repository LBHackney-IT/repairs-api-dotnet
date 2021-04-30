using System;

namespace RepairsApi.V2.Services
{
    public class DrsOptions
    {
        public Uri APIAddress { get; set; }
        public Uri ManagementAddress { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
