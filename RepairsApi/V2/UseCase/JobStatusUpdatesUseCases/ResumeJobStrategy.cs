using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class ResumeJobStrategy : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;

        public ResumeJobStrategy(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanResumeJob();

            workOrder.StatusCode = WorkStatusCode.Open;

            await _repairsGateway.SaveChangesAsync();
        }
    }
}
