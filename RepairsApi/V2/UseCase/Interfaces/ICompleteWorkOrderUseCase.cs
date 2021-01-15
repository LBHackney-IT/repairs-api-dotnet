using System.Threading.Tasks;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface ICompleteWorkOrderUseCase
    {
        Task<bool> Execute(WorkOrderComplete request);
    }

}
