using Microsoft.Extensions.Options;
using RepairsApi.V1.Domain;
using RepairsApi.V1.Factories;
using RepairsApi.V1.Gateways.Models;
using RepairsApi.V1.UseCase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public class PropertyGateway : IPropertyGateway
    {
        private readonly GatewayOptions _options;
        private readonly IApiGateway _apiGateway;

        public PropertyGateway(IOptions<GatewayOptions> options, IApiGateway apiGateway)
        {
            _options = options.Value;
            _apiGateway = apiGateway;
        }

        public async Task<IEnumerable<PropertyModel>> GetByQueryAsync(PropertySearchModel searchModel)
        {
            Uri url = new Uri(_options.PropertiesAPI + $"properties?{searchModel.GetQueryParameter()}");
            var response = await _apiGateway.ExecuteRequest<List<PropertyApiResponse>>(url);

            return response.Content.ToDomain();
        }

        public async Task<PropertyModel> GetByReferenceAsync(string propertyReference)
        {
            Uri url = new Uri(_options.PropertiesAPI + $"properties/{propertyReference}");
            var response = await _apiGateway.ExecuteRequest<PropertyApiResponse>(url);

            return response.Content.ToDomain();
        }
    }
}
