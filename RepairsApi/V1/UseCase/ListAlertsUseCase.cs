using RepairsApi.V1.Domain;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
    public class ListAlertsUseCase : IListAlertsUseCase
    {
        private readonly IPropertyGateway _propertyGateway;

        public ListAlertsUseCase(IPropertyGateway propertyGateway)
        {
            _propertyGateway = propertyGateway;
        }

        public Task<PropertyAlertList> ExecuteAsync(string propertyReference)
        {
            return _propertyGateway.GetAlertsAsync(propertyReference);
        }
    }
}
