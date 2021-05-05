using RepairsApi.V2.Configuration;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IGetFilterUseCase
    {
        Task<ModelFilterConfiguration> Execute(string modelName);
    }
}
