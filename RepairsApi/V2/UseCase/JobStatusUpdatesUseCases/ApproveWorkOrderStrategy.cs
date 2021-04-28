using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class ApproveWorkOrderStrategy : IJobStatusUpdateStrategy
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepairsGateway _repairsGateway;
        private readonly INotifier _notifier;

        public ApproveWorkOrderStrategy(
            ICurrentUserService currentUserService,
            IRepairsGateway repairsGateway,
            INotifier notifier)
        {
            _currentUserService = currentUserService;
            _repairsGateway = repairsGateway;
            _notifier = notifier;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.VerifyCanApproveWorkOrder();

            if (!_currentUserService.HasGroup(UserGroups.AuthorisationManager))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            workOrder.StatusCode = Infrastructure.WorkStatusCode.Open;

            await NotifyHandlers(workOrder);
        }

        private async Task NotifyHandlers(Infrastructure.WorkOrder workOrder)
        {
            var notification = new WorkOrderOpened(workOrder);
            await _notifier.Notify(notification);
        }
    }
}
