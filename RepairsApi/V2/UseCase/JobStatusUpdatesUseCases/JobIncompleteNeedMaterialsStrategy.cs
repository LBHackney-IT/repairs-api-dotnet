using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class JobIncompleteNeedMaterialsStrategy : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;

        public JobIncompleteNeedMaterialsStrategy(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanMoveToJobIncomplete();

            workOrder.StatusCode = Infrastructure.WorkStatusCode.PendMaterial;

            await _repairsGateway.SaveChangesAsync();
        }
    }
}
