using Microsoft.AspNetCore.Authorization;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System.Threading.Tasks;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class RejectVariationUseCase : IMoreSpecificSorUseCase, IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ICurrentUserService _currentUserService;

        public RejectVariationUseCase(IRepairsGateway repairsGateway,
            ICurrentUserService currentUserService)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.StatusCode = WorkStatusCode.Hold;
            workOrder.Reason = ReasonCode.NoApproval;
            await _repairsGateway.SaveChangesAsync();
            //if (_currentUserService.HasGroup(UserGroups.CONTRACT_MANAGER))
            //{
            //}
        }
    }
}
