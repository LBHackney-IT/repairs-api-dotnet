using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    internal class RejectWorkOrderStrategy : IJobStatusUpdateStrategy
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepairsGateway _repairsGateway;

        public RejectWorkOrderStrategy(ICurrentUserService currentUserService, IRepairsGateway repairsGateway)
        {
            _currentUserService = currentUserService;
            _repairsGateway = repairsGateway;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (!_currentUserService.HasGroup(UserGroups.CONTRACT_MANAGER))
                throw new UnauthorizedAccessException("You do not have the correct permissions for this action");

            if (workOrder.StatusCode != Infrastructure.WorkStatusCode.PendingApproval)
                throw new NotSupportedException("Work order is not pending approval");

            workOrder.StatusCode = Infrastructure.WorkStatusCode.Open;
        }
    }
}
