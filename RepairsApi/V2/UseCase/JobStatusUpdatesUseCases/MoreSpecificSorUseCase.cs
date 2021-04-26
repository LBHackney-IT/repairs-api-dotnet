using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobStatusUpdateTypeCode = RepairsApi.V2.Generated.JobStatusUpdateTypeCode;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;
using RepairsApi.V2.Helpers;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class MoreSpecificSorUseCase : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFeatureManager _featureManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUpdateSorCodesUseCase _updateSorCodesUseCase;

        public MoreSpecificSorUseCase(IRepairsGateway repairsGateway,
            IAuthorizationService authorizationService,
            IFeatureManager featureManager,
            ICurrentUserService currentUserService,
            IUpdateSorCodesUseCase updateSorCodesUseCase)
        {
            _repairsGateway = repairsGateway;
            _authorizationService = authorizationService;
            _featureManager = featureManager;
            _currentUserService = currentUserService;
            _updateSorCodesUseCase = updateSorCodesUseCase;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workElement = jobStatusUpdate.MoreSpecificSORCode.ToDb();

            WorkOrder workOrder = await GetWorkOrder(jobStatusUpdate);
            workOrder.VerifyCanVary();

            var authorised = await _authorizationService.AuthorizeAsync(_currentUserService.GetUser(), jobStatusUpdate, "VarySpendLimit");

            if (await _featureManager.IsEnabledAsync(FeatureFlags.SpendLimits) && !authorised.Succeeded)
            {
                workOrder.StatusCode = WorkStatusCode.PendingVariation;
                jobStatusUpdate.TypeCode = JobStatusUpdateTypeCode._180;
            }
            else
            {
                await _updateSorCodesUseCase.Execute(workOrder, workElement);
            }

            await _repairsGateway.SaveChangesAsync();
        }

        private async Task<WorkOrder> GetWorkOrder(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            return workOrder;
        }
    }

}
