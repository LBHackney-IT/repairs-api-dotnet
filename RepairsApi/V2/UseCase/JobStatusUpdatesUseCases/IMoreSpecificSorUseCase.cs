using System.Threading.Tasks;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public interface IMoreSpecificSorUseCase
    {
        Task Execute(JobStatusUpdate jobStatusUpdate);
    }
}
