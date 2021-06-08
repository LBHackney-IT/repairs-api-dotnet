using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class AssignOperativesUseCase : IJobStatusUpdateStrategy
    {
        private readonly IOperativesGateway _operativesGateway;

        public AssignOperativesUseCase(IOperativesGateway operativesGateway)
        {
            _operativesGateway = operativesGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanAssignOperative();

            var operativeIds = jobStatusUpdate.OperativesAssigned.Select(oa => int.Parse(oa.Identification.Number));

            await _operativesGateway.AssignOperatives(operativeIds.Select(o => new WorkOrderOperative { OperativeId = o, WorkOrderId = workOrder.Id }).ToArray());
        }
    }
}
