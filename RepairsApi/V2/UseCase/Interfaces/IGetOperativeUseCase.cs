using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IGetOperativeUseCase
    {
        Task<OperativeResponse> ExecuteAsync(string operativePrn);
    }
}
