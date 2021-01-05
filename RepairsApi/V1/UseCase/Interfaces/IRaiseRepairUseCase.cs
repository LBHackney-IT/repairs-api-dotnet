using RepairsApi.V1.Domain.Repair;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase.Interfaces
{
    public interface IRaiseRepairUseCase
    {
        Task<bool> Execute(WorkOrder raiseRepair);
    }
}
