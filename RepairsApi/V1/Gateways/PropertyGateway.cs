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
        private readonly IApiGateway _apiGateway;
        private readonly ILogger<PropertyGateway> _logger;

        public PropertyGateway(IApiGateway apiGateway, ILogger<PropertyGateway> logger)
        {
            _apiGateway = apiGateway;
            _logger = logger;
        }

        public async Task<IEnumerable<PropertyModel>> GetByQueryAsync(PropertySearchModel searchModel)
        {
            Uri url = new Uri($"properties?{searchModel.GetQueryParameter()}", UriKind.Relative);
            var response = await _apiGateway.ExecuteRequest<List<PropertyApiResponse>>(HttpClientNames.Properties, url);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.PropertiesFailure);
            }

            return response.Content.ToDomain();
        }

        public async Task<PropertyModel> GetByReferenceAsync(string propertyReference)
        {
            Uri url = new Uri($"properties/{propertyReference}", UriKind.Relative);
            var response = await _apiGateway.ExecuteRequest<PropertyApiResponse>(HttpClientNames.Properties, url);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.PropertyFailure);
            }

            return response.Content.ToDomain();
        }
    }
}
