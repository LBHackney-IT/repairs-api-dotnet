using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthorizationService _authorizationService;
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;

        public ApproveVariationUseCase(IRepairsGateway repairsGateway, IJobStatusUpdateGateway jobStatusUpdateGateway,
            ICurrentUserService currentUserService, IUpdateSorCodesUseCase updateSorCodesUseCase, IAuthorizationService authorizationService)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
            _updateSorCodesUseCase = updateSorCodesUseCase;
            _authorizationService = authorizationService;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            if (!_currentUserService.HasGroup(UserGroups.ContractManager)) throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            WorkOrder workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanApproveVariation();

            var variationJobStatus = await _jobStatusUpdateGateway.GetOutstandingVariation(workOrder.Id);

            var authorised = await _authorizationService.AuthorizeAsync(_currentUserService.GetUser(), variationJobStatus, "VarySpendLimit");

            if (!authorised.Succeeded) throw new UnauthorizedAccessException("Cannot Approve a Work Order Above Spend Limit");

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
