using RepairsApi.V2.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
#nullable enable
    public interface IListPropertiesUseCase
    {
        Task<IEnumerable<PropertyModel>> ExecuteAsync(PropertySearchModel searchModel);
    }
}
