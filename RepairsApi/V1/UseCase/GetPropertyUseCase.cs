using RepairsApi.V1.Domain;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
    public class GetPropertyUseCase : IGetPropertyUseCase
    {
        private readonly IPropertyGateway _propertyGateway;
        private readonly IAlertsGateway _alertsGateway;
        private readonly ITenancyGateway _tenancyGateway;

        public GetPropertyUseCase(IPropertyGateway propertyGateway, IAlertsGateway alertsGateway, ITenancyGateway tenancyGateway)
        {
            _propertyGateway = propertyGateway;
            _alertsGateway = alertsGateway;
            _tenancyGateway = tenancyGateway;
        }

        public async Task<PropertyWithAlerts> ExecuteAsync(string propertyReference)
        {
            var property = await _propertyGateway.GetByReferenceAsync(propertyReference);

            if (property is null) return null;

            var locationAlertList = await _alertsGateway.GetLocationAlertsAsync(propertyReference);
            var tenureInformation = await _tenancyGateway.GetTenancyInformationAsync(propertyReference);
            var personAlertList = await _alertsGateway.GetPersonAlertsAsync(tenureInformation?.TenancyAgreementReference);

            return new PropertyWithAlerts
            {
                PropertyModel = property,
                LocationAlerts = locationAlertList.Alerts,
                PersonAlerts = personAlertList.Alerts,
                Tenure = tenureInformation
            };
        }
    }
}
