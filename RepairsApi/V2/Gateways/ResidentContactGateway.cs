using Microsoft.Extensions.Logging;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Factories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways.Models;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public class ResidentContactGateway : IResidentContactGateway
    {
        private readonly IApiGateway _apiGateway;
        private readonly ILogger<ResidentContactGateway> _logger;

        public ResidentContactGateway(IApiGateway apiGateway, ILogger<ResidentContactGateway> logger)
        {
            _apiGateway = apiGateway;
            _logger = logger;
        }

        public async Task<IEnumerable<ResidentContact>> GetByHouseholdReferenceAsync(string householdReference)
        {
            Uri url = new Uri($"households?house_reference={householdReference}&active_tenancies_only=true", UriKind.Relative);
            var response = await _apiGateway.ExecuteRequest<HousingResidentInformationApiResponse>(HttpClientNames.Contacts, url);

            if (response.Status == HttpStatusCode.NotFound)
            {
                throw new ResourceNotFoundException(Resources.Contacts_Not_Found);
            }

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to {url} failed with {response.Status}");
                throw new ApiException(response.Status, Resources.ContactsFailure);
            }

            return response.Content.ToDomain();
        }
    }
}
