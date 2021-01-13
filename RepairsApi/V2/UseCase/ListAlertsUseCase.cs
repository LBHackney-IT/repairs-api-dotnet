using RepairsApi.V2.Domain;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
#nullable enable
    public class ListAlertsUseCase : IListAlertsUseCase
    {
        private readonly IAlertsGateway _alertsGateway;
        private readonly ITenancyGateway _tenancyGateway;

        public ListAlertsUseCase(IAlertsGateway alertsGateway, ITenancyGateway tenancyGateway)
        {
            _alertsGateway = alertsGateway;
            _tenancyGateway = tenancyGateway;
        }

        public async Task<AlertList> ExecuteAsync(string propertyReference)
        {
            PropertyAlertList propertyAlertList = await _alertsGateway.GetLocationAlertsAsync(propertyReference);
            var tenureInformation = await _tenancyGateway.GetTenancyInformationAsync(propertyReference);
            var personAlertList = await _alertsGateway.GetPersonAlertsAsync(tenureInformation?.TenancyAgreementReference);
            return new AlertList
            {
                PropertyAlerts = propertyAlertList,
                PersonAlerts = personAlertList
            };
        }
    }
}
