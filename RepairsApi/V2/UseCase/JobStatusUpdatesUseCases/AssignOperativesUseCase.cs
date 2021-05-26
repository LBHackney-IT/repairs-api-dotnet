using System.Linq;
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

            var payrollNumbers = jobStatusUpdate.OperativesAssigned.Select(oa => oa.Identification.Number);

            return Task.CompletedTask;
        }
    }
}
