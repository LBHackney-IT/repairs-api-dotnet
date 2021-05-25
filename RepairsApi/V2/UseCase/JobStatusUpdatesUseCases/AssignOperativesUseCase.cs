using System.Threading.Tasks;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class AssignOperativesUseCase : IJobStatusUpdateStrategy
    {
        public Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanAssignOperative();

            workOrder.AssignedOperatives = jobStatusUpdate.OperativesAssigned;

            return Task.CompletedTask;
        }
    }
}
