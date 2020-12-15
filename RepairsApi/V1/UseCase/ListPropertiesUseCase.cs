using RepairsApi.V1.Domain;
using RepairsApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
    public class ListPropertiesUseCase : IListPropertiesUseCase
    {
        public async Task<IEnumerable<PropertyModel>> ExecuteAsync(PropertySearchModel searchModel)
        {
            return await Task.FromResult(new List<PropertyModel>()).ConfigureAwait(false);
        }
    }
}
