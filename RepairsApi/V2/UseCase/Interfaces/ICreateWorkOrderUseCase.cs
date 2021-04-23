using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface ICreateWorkOrderUseCase
    {
        Task<CreateOrderResult> Execute(WorkOrder raiseRepair);
    }

}
