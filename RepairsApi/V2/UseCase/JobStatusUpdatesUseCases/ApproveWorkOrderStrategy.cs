using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
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
        private readonly INotifier _notifier;

        public ApproveWorkOrderStrategy(
            ICurrentUserService currentUserService,
            INotifier notifier)
        {
            _currentUserService = currentUserService;
            _notifier = notifier;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanApproveWorkOrder();

            if (!_currentUserService.HasGroup(UserGroups.AuthorisationManager))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            workOrder.StatusCode = WorkStatusCode.Open;
            jobStatusUpdate.Comments = $"{jobStatusUpdate.Comments} Approved By: {_currentUserService.GetHubUser().Name}";

            await NotifyHandlers(workOrder);
        }

        private async Task NotifyHandlers(Infrastructure.WorkOrder workOrder)
        {
            var notification = new WorkOrderOpened(workOrder);
            await _notifier.Notify(notification);

            await _notifier.Notify(new WorkOrderApproved(workOrder));
        }
    }
}
