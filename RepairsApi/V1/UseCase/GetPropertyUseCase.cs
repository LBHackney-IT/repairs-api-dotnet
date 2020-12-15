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

        public Task<PropertyWithAlerts> ExecuteAsync(string propertyReference)
        {
            return _propertyGateway.GetByReferenceAsync(propertyReference, includeAlerts: false);
        }
    }
}
