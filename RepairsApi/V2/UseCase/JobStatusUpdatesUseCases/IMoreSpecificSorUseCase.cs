using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public interface IMoreSpecificSorUseCase
    {
        Task Execute(WorkElement workElement, WorkOrder workOrder);
    }
}
