using RepairsApi.V1.Domain;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
    public class GetPropertyUseCase : IGetPropertyUseCase
    {
        private readonly IPropertyGateway _propertyGateway;

        public GetPropertyUseCase(IPropertyGateway propertyGateway)
        {
            _propertyGateway = propertyGateway;
        }

        public async Task<PropertyWithAlerts> ExecuteAsync(string propertyReference)
        {
            var property = await _propertyGateway.GetByReferenceAsync(propertyReference).ConfigureAwait(false);
            var alertList = await _propertyGateway.GetAlertsAsync(propertyReference).ConfigureAwait(false);

            return new PropertyWithAlerts
            {
                PropertyModel = property,
                Alerts = alertList.Alerts
            };
        }
    }
}
