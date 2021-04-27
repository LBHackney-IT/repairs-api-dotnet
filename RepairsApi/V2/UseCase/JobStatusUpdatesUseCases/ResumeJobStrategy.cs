using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Helpers;
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
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.VerifyCanResumeJob();

            workOrder.StatusCode = Infrastructure.WorkStatusCode.Open;

            await _repairsGateway.SaveChangesAsync();
        }
    }
}
