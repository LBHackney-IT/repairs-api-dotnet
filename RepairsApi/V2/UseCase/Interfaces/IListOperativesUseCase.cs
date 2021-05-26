using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IListOperativesUseCase
    {
        Task<List<OperativeResponse>> ExecuteAsync(Boundary.Request.OperativeRequest searchModel);
    }
}
