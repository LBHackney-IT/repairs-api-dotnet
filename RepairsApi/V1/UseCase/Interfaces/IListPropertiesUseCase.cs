using RepairsApi.V1.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase.Interfaces
{
#nullable enable
    public interface IListPropertiesUseCase
    {
        Task<IEnumerable<PropertyModel>> ExecuteAsync(PropertySearchModel searchModel);
    }
}
