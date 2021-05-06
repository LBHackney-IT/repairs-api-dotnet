using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Helpers;
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
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.VerifyCanMoveToJobIncomplete();

            workOrder.StatusCode = Infrastructure.WorkStatusCode.Hold;

            await _repairsGateway.SaveChangesAsync();
        }
    }
}
