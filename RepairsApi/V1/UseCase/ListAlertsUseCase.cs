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

        public Task<PropertyAlertList> ExecuteAsync(string propertyReference)
        {
            return _alertsGateway.GetAlertsAsync(propertyReference);
        }
    }
}
