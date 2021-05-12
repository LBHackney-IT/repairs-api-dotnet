using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class JobIncompleteStrategy : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;

        public JobIncompleteStrategy(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanMoveToJobIncomplete();

            workOrder.StatusCode = WorkStatusCode.Hold;

            await _repairsGateway.SaveChangesAsync();
        }
    }
}
