using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class RejectVariationUseCase : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ICurrentUserService _currentUserService;

        public RejectVariationUseCase(
            IRepairsGateway repairsGateway,
            ICurrentUserService currentUserService)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.VerifyCanRejectVariation();

            if (!_currentUserService.HasGroup(UserGroups.ContractManager))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            workOrder.StatusCode = WorkStatusCode.VariationRejected;
            if (jobStatusUpdate.Comments is null || !jobStatusUpdate.Comments.Contains(Resources.RejectedVariationPrepend))
            {
                jobStatusUpdate.Comments = $"{Resources.RejectedVariationPrepend}{jobStatusUpdate.Comments}";
            }
            await _repairsGateway.SaveChangesAsync();
        }
    }
}
