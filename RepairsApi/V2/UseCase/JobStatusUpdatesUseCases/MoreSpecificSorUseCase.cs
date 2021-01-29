using System.Threading.Tasks;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class MoreSpecificSorUseCase : IMoreSpecificSorUseCase
    {
        private readonly IRepairsGateway _repairsGateway;

        public MoreSpecificSorUseCase(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);
            var workElement = jobStatusUpdate.MoreSpecificSORCode.ToDb();

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            workOrder.WorkElements.AssertForEach():

            await _repairsGateway.AddWorkElement(workOrderId, workElement);
        }
    }

}
