using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;


namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class ApproveVariationUseCase : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUpdateSorCodesUseCase _updateSorCodesUseCase;
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;

        public ApproveVariationUseCase(IRepairsGateway repairsGateway, IJobStatusUpdateGateway jobStatusUpdateGateway,
            ICurrentUserService currentUserService, IUpdateSorCodesUseCase updateSorCodesUseCase)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
            _updateSorCodesUseCase = updateSorCodesUseCase;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            if (!_currentUserService.HasGroup(UserGroups.ContractManager)) throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            WorkOrder workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanApproveVariation();


            var variationJobStatus = await _jobStatusUpdateGateway.GetOutstandingVariation(workOrder.Id);

            await VaryWorkOrder(workOrder, variationJobStatus);

            jobStatusUpdate.Comments = $"{jobStatusUpdate.Comments} Approved By: {_currentUserService.GetHubUser().Name}";
            await _repairsGateway.SaveChangesAsync();
        }

        private async Task VaryWorkOrder(WorkOrder workOrder, Infrastructure.JobStatusUpdate variationJobStatus)
        {
            await _updateSorCodesUseCase.Execute(workOrder, variationJobStatus.MoreSpecificSORCode);
            workOrder.StatusCode = WorkStatusCode.VariationApproved;
        }
    }
}
