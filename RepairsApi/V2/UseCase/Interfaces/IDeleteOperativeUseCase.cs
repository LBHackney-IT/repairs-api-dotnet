using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IDeleteOperativeUseCase
    {
        Task<bool> ExecuteAsync(string operativePrn);
    }
}
