using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface ICreateWorkOrderUseCase
    {
        Task<int> Execute(WorkOrder raiseRepair);
    }

}
