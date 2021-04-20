using RepairsApi.V2.Boundary;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IGetVariationTasksUseCase
    {
        Task<WorkOrderResponse> Execute(int id);
    }
}
