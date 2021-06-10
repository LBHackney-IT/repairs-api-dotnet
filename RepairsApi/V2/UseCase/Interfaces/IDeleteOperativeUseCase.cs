using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IDeleteOperativeUseCase
    {
        Task ExecuteAsync(string operativePrn);
    }
}
