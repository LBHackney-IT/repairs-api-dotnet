using RepairsApi.V2.Domain;
using RepairsApi.V2.UseCase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public interface IPropertyGateway
    {
        Task<PropertyModel> GetByReferenceAsync(string propertyReference);
        Task<IEnumerable<PropertyModel>> GetByQueryAsync(PropertySearchModel searchModel);
    }
}
