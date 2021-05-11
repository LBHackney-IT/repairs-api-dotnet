using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobStatusUpdateTypeCode = RepairsApi.V2.Generated.JobStatusUpdateTypeCode;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class MoreSpecificSorUseCase : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFeatureManager _featureManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUpdateSorCodesUseCase _updateSorCodesUseCase;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public MoreSpecificSorUseCase(IRepairsGateway repairsGateway,
            IAuthorizationService authorizationService,
            IFeatureManager featureManager,
            ICurrentUserService currentUserService,
            IUpdateSorCodesUseCase updateSorCodesUseCase,
            IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _repairsGateway = repairsGateway;
            _authorizationService = authorizationService;
            _featureManager = featureManager;
            _currentUserService = currentUserService;
            _updateSorCodesUseCase = updateSorCodesUseCase;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            WorkOrder workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanVary();

            var workElement = jobStatusUpdate.MoreSpecificSORCode;
            await AddCodeCosts(workElement.RateScheduleItem, workOrder.AssignedToPrimary?.ContractorReference);

            var authorised = await _authorizationService.AuthorizeAsync(_currentUserService.GetUser(), jobStatusUpdate, "VarySpendLimit");

            if (await _featureManager.IsEnabledAsync(FeatureFlags.SpendLimits) && !authorised.Succeeded)
            {
                workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
                jobStatusUpdate.TypeCode = JobStatusUpdateTypeCode._180;
            }
            else
            {
                await _updateSorCodesUseCase.Execute(workOrder, workElement);
            }

            jobStatusUpdate.PrefixComments(Resources.VariationReason);

            await _repairsGateway.SaveChangesAsync();
        }

        private async Task AddCodeCosts(IEnumerable<RateScheduleItem> newCodes, string contractorReference)
        {
            foreach (var newCode in newCodes)
            {
                newCode.Original = false;
                newCode.CodeCost = await _scheduleOfRatesGateway.GetCost(contractorReference, newCode.CustomCode);
            }
        }
    }

}
