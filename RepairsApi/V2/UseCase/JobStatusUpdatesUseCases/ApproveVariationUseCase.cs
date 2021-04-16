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
        private readonly IMoreSpecificSorUseCase _specificSorUseCase;
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;

        public ApproveVariationUseCase(IRepairsGateway repairsGateway, IJobStatusUpdateGateway jobStatusUpdateGateway,
            ICurrentUserService currentUserService, IMoreSpecificSorUseCase specificSorUseCase)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
            _specificSorUseCase = specificSorUseCase;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (!_currentUserService.HasGroup(UserGroups.CONTRACT_MANAGER))
                throw new UnauthorizedAccessException("You do not have the correct permissions for this action");

            var variationJobStatus = await _jobStatusUpdateGateway.SelectLastJobStatusUpdate
                (Generated.JobStatusUpdateTypeCode._180, workOrderId);

            await _specificSorUseCase.Execute(variationJobStatus.MoreSpecificSORCode, workOrder);
            jobStatusUpdate.Comments = $"{jobStatusUpdate.Comments} Approved By: {_currentUserService.GetHubUser().Name}";

            workOrder.StatusCode = WorkStatusCode.VariationApproved;
            await _repairsGateway.SaveChangesAsync();
        }

    }
}
