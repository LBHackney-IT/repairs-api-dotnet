using System.Collections.Generic;

namespace RepairsApi.V1.Gateways.Models
{
    public class ListTenanciesApiResponse
    {
        public List<TenancyApiTenancyInformation> Tenancies { get; set; }
    }
}
