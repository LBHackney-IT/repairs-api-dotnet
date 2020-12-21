using Microsoft.Extensions.Options;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public class TenancyGateway : ITenancyGateway
    {
        private readonly GatewayOptions _options;
        private readonly IApiGateway _apiGateway;

        public TenancyGateway(IOptions<GatewayOptions> options, IApiGateway apiGateway)
        {
            _options = options.Value;
            _apiGateway = apiGateway;
        }

        public async Task<TenureInformation> GetTenancyInformationAsync(string propertyReference)
        {
            Uri url = new Uri(_options.TenancyApi + $"tenancies?property_reference={propertyReference}");

            var response = await _apiGateway.ExecuteRequest<ListTenanciesApiResponse>(url);

            return response.Content.ToDomain();
        }
    }
}
