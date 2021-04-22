using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
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
        private readonly IList<WorkStatusCode> _authorizedStatus;

        public ContractorAcknowledgeVariationUseCase(IRepairsGateway repairsGateway,
            ICurrentUserService currentUserService)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;

            _authorizedStatus = new List<WorkStatusCode>
            {
                WorkStatusCode.VariationApproved,
                WorkStatusCode.VariationRejected
            };
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (!_currentUserService.HasGroup(UserGroups.Contractor))
                throw new UnauthorizedAccessException("You do not have the correct permissions for this action");

            if (!_authorizedStatus.Contains(workOrder.StatusCode))
                throw new NotSupportedException("This action is not supported");

            workOrder.StatusCode = WorkStatusCode.Open;
            await _repairsGateway.SaveChangesAsync();

        }
    }
}
