using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public class RejectWorkOrderStrategy : IJobStatusUpdateStrategy
    {
        private readonly ICurrentUserService _currentUserService;

        public RejectWorkOrderStrategy(
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanRejectWorkOrder();

            if (!_currentUserService.HasGroup(UserGroups.AuthorisationManager))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            workOrder.StatusCode = WorkStatusCode.Canceled;
            jobStatusUpdate.PrefixComments(Resources.WorkOrderAuthorisationRejected);

            return Task.CompletedTask;
        }
    }
}
