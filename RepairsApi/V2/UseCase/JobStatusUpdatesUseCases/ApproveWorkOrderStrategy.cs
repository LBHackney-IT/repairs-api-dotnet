using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthorizationService _authorizationService;

        public ApproveWorkOrderStrategy(
            ICurrentUserService currentUserService,
            INotifier notifier,
            IAuthorizationService authorizationService)
        {
            _currentUserService = currentUserService;
            _notifier = notifier;
            _authorizationService = authorizationService;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanApproveWorkOrder();

            if (!_currentUserService.HasGroup(UserGroups.AuthorisationManager))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            var authorised = await _authorizationService.AuthorizeAsync(_currentUserService.GetUser(), workOrder, "RaiseSpendLimit");

            if (!authorised.Succeeded) throw new UnauthorizedAccessException("Cannot Approve a work order above spend limit");

            workOrder.StatusCode = WorkStatusCode.Open;
            jobStatusUpdate.Comments = $"{jobStatusUpdate.Comments} Approved By: {_currentUserService.GetHubUser().Name}";

            await NotifyHandlers(workOrder);
        }

        private async Task NotifyHandlers(Infrastructure.WorkOrder workOrder)
        {
            var notification = new WorkOrderOpened(workOrder);
            await _notifier.Notify(notification);
        }
    }
}
