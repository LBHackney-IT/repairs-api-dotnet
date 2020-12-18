using Microsoft.Extensions.Options;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task<PropertyAlertList> GetLocationAlertsAsync(string propertyReference)
        {
            Uri url = new Uri(_options.AlertsApi + $"cautionary-alerts/properties/{propertyReference}");

            var response = await _apiGateway.ExecuteRequest<PropertyAlertsApiResponse>(url);

            if (response.Status == HttpStatusCode.NotFound)
            {
                return EmptyPropertyAlertList(propertyReference);
            }

            return response.Content.ToDomain();
        }

        public async Task<PersonAlertList> GetPersonAlertsAsync(string tenancyReference)
        {
            if (tenancyReference == null)
            {
                return EmptyPersonAlertList();
            }

            Uri url = new Uri(_options.AlertsApi + $"cautionary-alerts/people?tag_ref={tenancyReference}");

            var response = await _apiGateway.ExecuteRequest<ListPersonAlertsApiResponse>(url);

            return response.Content.ToDomain();
        }

        private static PersonAlertList EmptyPersonAlertList()
        {
            return new PersonAlertList
            {
                Alerts = new List<Alert>()
            };
        }

        private static PropertyAlertList EmptyPropertyAlertList(string propertyReference)
        {
            return new PropertyAlertList
            {
                Alerts = new List<Alert>(),
                PropertyReference = propertyReference
            };
        }
    }
}
