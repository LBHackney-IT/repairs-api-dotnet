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

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class MoreSpecificSorUseCase : IMoreSpecificSorUseCase, IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFeatureManager _featureManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public MoreSpecificSorUseCase(IRepairsGateway repairsGateway,
            IAuthorizationService authorizationService,
            IFeatureManager featureManager,
            ICurrentUserService currentUserService,
            IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _repairsGateway = repairsGateway;
            _authorizationService = authorizationService;
            _featureManager = featureManager;
            _currentUserService = currentUserService;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);
            var workElement = jobStatusUpdate.MoreSpecificSORCode.ToDb();

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            var authorised = await _authorizationService.AuthorizeAsync(_currentUserService.GetUser(), jobStatusUpdate, "VarySpendLimit");

            //The workorder already has a variation
            if (workOrder.StatusCode == WorkStatusCode.VariationPendingApproval &&
                jobStatusUpdate.TypeCode == JobStatusUpdateTypeCode._80)
                throw new InvalidOperationException("This action is not permitted");

            if (await _featureManager.IsEnabledAsync(FeatureFlags.SpendLimits) && !authorised.Succeeded)
            {
                workOrder.StatusCode = WorkStatusCode.VariationPendingApproval;
                jobStatusUpdate.TypeCode = JobStatusUpdateTypeCode._180;
            }
            //User authorised, patch SOR codes
            else
                await Execute(workElement, workOrder);

            await _repairsGateway.SaveChangesAsync();
        }

        public async Task Execute(WorkElement workElement, WorkOrder workOrder)
        {
            var existingCodes = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem);
            var newCodes = workElement.RateScheduleItem.Where(rsi => !existingCodes.Any(ec => ec.Id == rsi.OriginalId));

            UpdateExistingCodes(existingCodes, workElement);
            await AddNewCodes(newCodes, workOrder);
        }

        private async Task AddNewCodes(IEnumerable<RateScheduleItem> newCodes, WorkOrder workOrder)
        {
            foreach (var newCode in newCodes)
            {
                newCode.Original = false;
                newCode.CodeCost = await _scheduleOfRatesGateway.GetCost(workOrder.AssignedToPrimary?.ContractorReference, newCode.CustomCode);
                workOrder.WorkElements.First().RateScheduleItem.Add(newCode);
            }
        }

        private static void UpdateExistingCodes(IEnumerable<RateScheduleItem> existingCodes, WorkElement workElement)
        {

            foreach (var existingCode in existingCodes)
            {
                var updatedCode = workElement.RateScheduleItem.SingleOrDefault(rsi => rsi.OriginalId == existingCode.Id);
                if (updatedCode == null)
                {
                    throw new NotSupportedException($"Deleting SOR codes not supported, missing {existingCode.CustomCode}");
                }

                existingCode.Quantity.Amount = updatedCode.Quantity.Amount;
            }
        }
    }

}
