using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public interface IJobStatusUpdateStrategyFactory
    {
        Task ProcessActions(JobStatusUpdate jobStatusUpdate);
    }
}
