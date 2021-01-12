using RepairsApi.V1.Domain;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
#nullable enable
    public class ListPropertiesUseCase : IListPropertiesUseCase
    {
        private readonly IPropertyGateway _propertyGateway;

        public ListPropertiesUseCase(IPropertyGateway propertyGateway)
        {
            _propertyGateway = propertyGateway;
        }

        public async Task<IEnumerable<PropertyModel>> ExecuteAsync(PropertySearchModel searchModel)
        {
            if (!searchModel.IsValid())
            {
                return EmptyList();
            }

            return await _propertyGateway.GetByQueryAsync(searchModel);
        }

        private static List<PropertyModel> EmptyList()
        {
            return new List<PropertyModel>();
        }
    }
}
