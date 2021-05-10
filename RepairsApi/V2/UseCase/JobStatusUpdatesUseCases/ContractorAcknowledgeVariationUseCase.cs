using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class ContractorAcknowledgeVariationUseCase : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ICurrentUserService _currentUserService;

        public ContractorAcknowledgeVariationUseCase(IRepairsGateway repairsGateway,
            ICurrentUserService currentUserService)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.VerifyCanAcknowledgeVariation();

            if (!_currentUserService.HasGroup(UserGroups.Contractor))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            workOrder.StatusCode = WorkStatusCode.Open;
            await _repairsGateway.SaveChangesAsync();

        }
    }
}
