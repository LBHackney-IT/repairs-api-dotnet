using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Notifications;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class RejectWorkOrderStrategy : IJobStatusUpdateStrategy
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotifier _notifier;

        public RejectWorkOrderStrategy(
            ICurrentUserService currentUserService, INotifier notifier)
        {
            _currentUserService = currentUserService;
            _notifier = notifier;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanRejectWorkOrder();

            if (!_currentUserService.HasGroup(UserGroups.AuthorisationManager))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            workOrder.StatusCode = WorkStatusCode.Canceled;
            jobStatusUpdate.PrefixComments(Resources.WorkOrderAuthorisationRejected);

            await _notifier.Notify(new WorkOrderRejected(workOrder));
        }
    }
}
