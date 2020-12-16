using Microsoft.Extensions.Options;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public class AlertsGateway : IAlertsGateway
    {
        private readonly GatewayOptions _options;
        private readonly IApiGateway _apiGateway;

        public AlertsGateway(IOptions<GatewayOptions> options, IApiGateway apiGateway)
        {
            _options = options.Value;
            _apiGateway = apiGateway;
        }

        public async Task<PropertyAlertList> GetAlertsAsync(string propertyReference)
        {
            Uri url = new Uri(_options.AlertsApi + $"cautionary-alerts/properties/{propertyReference}");
            var response = await _apiGateway.ExecuteRequest<AlertsApiResponse>(url).ConfigureAwait(false);

            return response.ToDomain();
        }
    }
}
