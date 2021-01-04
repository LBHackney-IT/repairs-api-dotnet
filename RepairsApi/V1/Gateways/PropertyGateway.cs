using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Exceptions;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;
using RepairsApi.V1.UseCase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
#nullable enable
    public class PropertyGateway : IPropertyGateway
    {
        private readonly GatewayOptions _options;
        private readonly IApiGateway _apiGateway;
        private readonly ILogger<PropertyGateway> _logger;

        public PropertyGateway(IOptions<GatewayOptions> options, IApiGateway apiGateway, ILogger<PropertyGateway> logger)
        {
            _options = options.Value;
            _apiGateway = apiGateway;
            _logger = logger;
        }

        public async Task<IEnumerable<PropertyModel>> GetByQueryAsync(PropertySearchModel searchModel)
        {
            Uri url = new Uri(_options.PropertiesAPI + $"properties?{searchModel.GetQueryParameter()}");
            var response = await _apiGateway.ExecuteRequest<List<PropertyApiResponse>>(url, _options.PropertiesAPIKey);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.PropertiesFailure);
            }

            return response.Content.ToDomain();
        }

        public async Task<PropertyModel> GetByReferenceAsync(string propertyReference)
        {
            Uri url = new Uri(_options.PropertiesAPI + $"properties/{propertyReference}");
            var response = await _apiGateway.ExecuteRequest<PropertyApiResponse>(url, _options.PropertiesAPIKey);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.PropertyFailure);
            }

            return response.Content.ToDomain();
        }
    }
}
