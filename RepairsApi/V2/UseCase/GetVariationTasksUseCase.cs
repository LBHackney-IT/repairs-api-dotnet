using RepairsApi.V2.Boundary;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class GetVariationTasksUseCase : IGetVariationTasksUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        public GetVariationTasksUseCase(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task<WorkOrderResponse> Execute(int id)
        {
            WorkOrder workOrder = await _repairsGateway.GetWorkOrder(id);

            return new WorkOrderResponse();
;        }
    }
}
