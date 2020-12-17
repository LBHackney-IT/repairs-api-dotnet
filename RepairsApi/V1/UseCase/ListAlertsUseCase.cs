using RepairsApi.V1.Domain;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
    public class ListAlertsUseCase : IListAlertsUseCase
    {
        private readonly IAlertsGateway _alertsGateway;

        public ListAlertsUseCase(IAlertsGateway alertsGateway)
        {
            _alertsGateway = alertsGateway;
        }

        public async Task<AlertList> ExecuteAsync(string propertyReference)
        {
            PropertyAlertList propertyAlertList = await _alertsGateway.GetLocationAlertsAsync(propertyReference);
            return new AlertList
            {
                PropertyAlerts = propertyAlertList,
                PersonAlerts = null // TODO
            };
        }
    }
}
