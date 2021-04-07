using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
#nullable enable
    public class GetPropertyUseCase : IGetPropertyUseCase
    {
        private readonly IPropertyGateway _propertyGateway;
        private readonly IAlertsGateway _alertsGateway;
        private readonly ITenancyGateway _tenancyGateway;
        private readonly IResidentContactGateway _residentContactGateway;

        public GetPropertyUseCase(IPropertyGateway propertyGateway, IAlertsGateway alertsGateway, ITenancyGateway tenancyGateway, IResidentContactGateway residentContactGateway)
        {
            _propertyGateway = propertyGateway;
            _alertsGateway = alertsGateway;
            _tenancyGateway = tenancyGateway;
            _residentContactGateway = residentContactGateway;
        }

        public async Task<PropertyWithAlerts> ExecuteAsync(string propertyReference)
        {
            var property = await _propertyGateway.GetByReferenceAsync(propertyReference);
            var locationAlertList = await _alertsGateway.GetLocationAlertsAsync(propertyReference);
            var tenureInformation = await _tenancyGateway.GetTenancyInformationAsync(propertyReference);
            var personAlertList = await _alertsGateway.GetPersonAlertsAsync(tenureInformation?.TenancyAgreementReference);

            var residentContactList = (tenureInformation == null) ? Enumerable.Empty<ResidentContact>() :
                await _residentContactGateway.GetByHouseholdReferenceAsync(tenureInformation?.HouseholdReference);
            return new PropertyWithAlerts
            {
                PropertyModel = property,
                LocationAlerts = locationAlertList.Alerts,
                PersonAlerts = personAlertList.Alerts,
                Tenure = tenureInformation,
                Contacts = residentContactList
            };
        }
    }
}
