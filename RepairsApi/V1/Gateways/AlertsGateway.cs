using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Exceptions;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
#nullable enable
    public class AlertsGateway : IAlertsGateway
    {
        private readonly ILogger<AlertsGateway> _logger;
        private readonly IApiGateway _apiGateway;

        public AlertsGateway(IApiGateway apiGateway, ILogger<AlertsGateway> logger)
        {
            _logger = logger;
            _apiGateway = apiGateway;
        }

        public async Task<PropertyAlertList> GetLocationAlertsAsync(string propertyReference)
        {
            Uri url = new Uri($"cautionary-alerts/properties/{propertyReference}", UriKind.Relative);
            var response = await _apiGateway.ExecuteRequest<PropertyAlertsApiResponse>(HttpClientNames.Alerts, url);

            if (response.Status == HttpStatusCode.NotFound)
            {
                return EmptyPropertyAlertList(propertyReference);
            }

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.LocationAlertsFailure);
            }

            return response.Content.ToDomain();
        }

        public async Task<PersonAlertList> GetPersonAlertsAsync(string? tenancyReference)
        {
            if (tenancyReference == null)
            {
                return EmptyPersonAlertList();
            }

            Uri url = new Uri($"cautionary-alerts/people?tag_ref={tenancyReference}", UriKind.Relative);
            var response = await _apiGateway.ExecuteRequest<ListPersonAlertsApiResponse>(HttpClientNames.Alerts, url);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.PersonAlertsFailure);
            }

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
