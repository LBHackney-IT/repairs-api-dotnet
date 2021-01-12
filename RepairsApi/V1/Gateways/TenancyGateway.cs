using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Exceptions;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
#nullable enable
    public class TenancyGateway : ITenancyGateway
    {
        private readonly IApiGateway _apiGateway;
        private readonly ILogger<TenancyGateway> _logger;

        public TenancyGateway(IApiGateway apiGateway, ILogger<TenancyGateway> logger)
        {
            _apiGateway = apiGateway;
            _logger = logger;
        }

        public async Task<TenureInformation?> GetTenancyInformationAsync(string propertyReference)
        {
            Uri url = new Uri($"tenancies?property_reference={propertyReference}", UriKind.Relative);
            var response = await _apiGateway.ExecuteRequest<ListTenanciesApiResponse>(HttpClientNames.Tenancy, url);

            if (!response.IsSuccess && response.Status != HttpStatusCode.NotFound)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.TenancyFailure);
            }

            if (response.Status == HttpStatusCode.NotFound || response.Content!.Tenancies.Count == 0)
            {
                return null;
            }

            return response.Content.ToDomain();
        }
    }
}
