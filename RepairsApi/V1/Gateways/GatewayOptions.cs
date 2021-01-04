using System;
using System.ComponentModel.DataAnnotations;

namespace RepairsApi.V1.Gateways
{
    public class GatewayOptions
    {
        [Required]
        public Uri PropertiesAPI { get; set; }

        [Required]
        public string PropertiesAPIKey { get; set; }

        [Required]
        public Uri AlertsApi { get; set; }

        [Required]
        public string AlertsAPIKey { get; set; }

        [Required]
        public Uri TenancyApi { get; set; }

        [Required]
        public string TenancyApiKey { get; set; }
    }
}
