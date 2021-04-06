using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;


namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class ApproveVariationUseCase : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ICurrentUserService _currentUserService;

        public ApproveVariationUseCase(IRepairsGateway repairsGateway,
            ICurrentUserService currentUserService)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (!_currentUserService.HasGroup(UserGroups.CONTRACT_MANAGER))
                throw new UnauthorizedAccessException("You do not have the correct permissions for this action");

            workOrder.StatusCode = WorkStatusCode.VariationApproved;
            await _repairsGateway.SaveChangesAsync();
        }
    }
}
