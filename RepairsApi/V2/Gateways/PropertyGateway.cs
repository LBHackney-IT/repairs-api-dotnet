using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways.Models;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public class PropertyGateway : IPropertyGateway
    {
        private readonly IApiGateway _apiGateway;
        private readonly ILogger<PropertyGateway> _logger;
        private readonly RepairsContext _repairsContext;

        public PropertyGateway(IApiGateway apiGateway, RepairsContext repairsContext, ILogger<PropertyGateway> logger)
        {
            _apiGateway = apiGateway;
            _logger = logger;
            _repairsContext = repairsContext;
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

            if (response.Status == HttpStatusCode.NotFound)
            {
                throw new ResourceNotFoundException(Resources.Property_Not_Found);
            }

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.PropertyFailure);
            }

            string? tmo = _repairsContext.Company.Where(c => c.CoCode.Equals(response.Content.CompAvail))
                .SingleOrDefault()?.Name;

            return response.Content.ToDomain(tmo);
        }
    }
}
