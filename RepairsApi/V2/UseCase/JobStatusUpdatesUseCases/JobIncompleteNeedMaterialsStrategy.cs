using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
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
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            workOrder.StatusCode = Infrastructure.WorkStatusCode.PendMaterial;

            await _repairsGateway.SaveChangesAsync();
        }
    }
}