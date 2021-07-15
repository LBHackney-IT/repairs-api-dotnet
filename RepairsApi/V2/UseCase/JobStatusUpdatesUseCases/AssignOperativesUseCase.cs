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
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public AssignOperativesUseCase(IOperativesGateway operativesGateway, IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _operativesGateway = operativesGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanAssignOperative();

            if (!await workOrder.CanAssignOperative(_scheduleOfRatesGateway)) ThrowHelper.ThrowUnsupported(Resources.NonOperativeContractor);

            var operativeIds = jobStatusUpdate.OperativesAssigned.Select(oa => int.Parse(oa.Identification.Number));

            await _operativesGateway.AssignOperatives(workOrder.Id, OperativeAssignmentType.Manual, operativeIds.ToArray());
        }
    }
}
