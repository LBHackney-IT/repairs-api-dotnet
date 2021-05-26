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
    public class RejectVariationUseCase : IJobStatusUpdateStrategy
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;
        private readonly INotifier _notifier;

        public RejectVariationUseCase(
            IRepairsGateway repairsGateway,
            ICurrentUserService currentUserService,
            IJobStatusUpdateGateway jobStatusUpdateGateway,
            INotifier notifier)
        {
            _repairsGateway = repairsGateway;
            _currentUserService = currentUserService;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
            _notifier = notifier;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrder = jobStatusUpdate.RelatedWorkOrder;
            workOrder.VerifyCanRejectVariation();

            if (!_currentUserService.HasGroup(UserGroups.ContractManager))
                throw new UnauthorizedAccessException(Resources.InvalidPermissions);

            var variationJobStatus = await _jobStatusUpdateGateway.GetOutstandingVariation(workOrder.Id);

            workOrder.StatusCode = WorkStatusCode.VariationRejected;
            jobStatusUpdate.PrefixComments(Resources.RejectedVariationPrepend);

            await _notifier.Notify(new VariationRejected(variationJobStatus, jobStatusUpdate));

            await _repairsGateway.SaveChangesAsync();
        }
    }
}
