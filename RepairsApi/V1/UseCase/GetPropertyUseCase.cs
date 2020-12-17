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

        public GetPropertyUseCase(IPropertyGateway propertyGateway, IAlertsGateway alertsGateway)
        {
            _propertyGateway = propertyGateway;
            _alertsGateway = alertsGateway;
        }

        public async Task<PropertyWithAlerts> ExecuteAsync(string propertyReference)
        {
            var property = await _propertyGateway.GetByReferenceAsync(propertyReference);

            if (property is null)
            {
                return null;
            }

            var alertList = await _alertsGateway.GetAlertsAsync(propertyReference);

            return new PropertyWithAlerts
            {
                PropertyModel = property,
                Alerts = alertList.Alerts
            };
        }
    }
}
