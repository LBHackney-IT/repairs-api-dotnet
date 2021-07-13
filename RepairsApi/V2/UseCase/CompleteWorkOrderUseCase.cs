using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Notifications;

namespace RepairsApi.V2.UseCase
{
    public class CompleteWorkOrderUseCase : ICompleteWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IWorkOrderCompletionGateway _workOrderCompletionGateway;
        private readonly ITransactionManager _transactionManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotifier _notifier;
        private readonly IFeatureManager _featureManager;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public CompleteWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IWorkOrderCompletionGateway workOrderCompletionGateway,
            ITransactionManager transactionManager,
            ICurrentUserService currentUserService,
            INotifier notifier,
            IFeatureManager featureManager,
            IScheduleOfRatesGateway scheduleOfRatesGateway
        )
        {
            _repairsGateway = repairsGateway;
            _workOrderCompletionGateway = workOrderCompletionGateway;
            _transactionManager = transactionManager;
            _currentUserService = currentUserService;
            _notifier = notifier;
            _featureManager = featureManager;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task Execute(Generated.WorkOrderComplete workOrderCompleteRequest)
        {
            var workOrderId = int.Parse(workOrderCompleteRequest.WorkOrderReference.ID);

            if (await _workOrderCompletionGateway.IsWorkOrderCompleted(workOrderId))
            {
                throw new NotSupportedException(Resources.CannotCompleteWorkOrderTwice);
            }

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            ValidateRequest(workOrderCompleteRequest);
            var workOrderComplete = workOrderCompleteRequest.ToDb(workOrder, null);
            await using var transaction = await _transactionManager.Start();
            await UpdateWorkOrderStatus(workOrder, workOrderComplete);
            await _workOrderCompletionGateway.CreateWorkOrderCompletion(workOrderComplete);
            await transaction.Commit();
        }

        private async Task UpdateWorkOrderStatus(WorkOrder workOrder, WorkOrderComplete workOrderComplete)
        {
            if (workOrderComplete.JobStatusUpdates?.Count != 1) throw new NotSupportedException("Work order complete must contain a single update");

            var update = workOrderComplete.JobStatusUpdates.Single();

            switch (update.TypeCode)
            {
                case Generated.JobStatusUpdateTypeCode._0: //Other
                    await HandleCustomType(workOrder, update);
                    break;
                case Generated.JobStatusUpdateTypeCode._70: // Denied Access
                    workOrder.VerifyCanComplete();

                    if (!_currentUserService.HasGroup(UserGroups.Contractor) &&
                        !_currentUserService.HasGroup(UserGroups.ContractManager))
                        throw new UnauthorizedAccessException("Not Authorised to close jobs");

                    update.PrefixComments(Resources.WorkOrderNoAccessPrefix);
                    await _repairsGateway.UpdateWorkOrderStatus(workOrder.Id, WorkStatusCode.NoAccess);
                    await _notifier.Notify(new WorkOrderNoAccess(workOrder));
                    break;
                default: throw new NotSupportedException(Resources.UnsupportedWorkOrderUpdate);
            }

            workOrder.ClosedDate = update.EventTime;
        }

        private async Task HandleCustomType(WorkOrder workOrder, JobStatusUpdate update)
        {
            switch (update.OtherType)
            {
                case CustomJobStatusUpdates.Completed:
                    await VerifyCanComplete(workOrder);
                    await _repairsGateway.UpdateWorkOrderStatus(workOrder.Id, WorkStatusCode.Complete);
                    update.PrefixComments(Resources.WorkOrderCompletedPrefix);
                    await _notifier.Notify(new WorkOrderCompleted(workOrder, update));
                    break;
                case CustomJobStatusUpdates.Cancelled:
                    workOrder.VerifyCanCancel();

                    if (!_currentUserService.HasAnyGroup(UserGroups.Agent, UserGroups.ContractManager, UserGroups.AuthorisationManager))
                        throw new UnauthorizedAccessException("Not Authorised to cancel jobs");

                    update.PrefixComments(Resources.WorkOrderCancelledPrefix);
                    await _repairsGateway.UpdateWorkOrderStatus(workOrder.Id, WorkStatusCode.Canceled);
                    await _notifier.Notify(new WorkOrderCancelled(workOrder));
                    break;
                default: throw new NotSupportedException(Resources.UnsupportedWorkOrderUpdate);
            }
        }

        private async Task VerifyCanComplete(WorkOrder workOrder)
        {
            if (await workOrder.CanAssignOperative(_scheduleOfRatesGateway)
                    && await _featureManager.IsEnabledAsync(FeatureFlags.EnforceAssignedOperative)
                    && workOrder.AssignedOperatives.IsNullOrEmpty())
            {
                ThrowHelper.ThrowUnsupported(Resources.CannotCompleteWithNoOperative);
            }
            workOrder.VerifyCanComplete();

            if (!_currentUserService.HasGroup(UserGroups.Contractor) &&
                !_currentUserService.HasGroup(UserGroups.ContractManager))
                throw new UnauthorizedAccessException("Not Authorised to close jobs");
        }

        private static void ValidateRequest(Generated.WorkOrderComplete request)
        {
            if (!(request.JobStatusUpdates is null))
            {
                if (request.JobStatusUpdates.Any(jsu => jsu.RelatedWorkElementReference != null && jsu.RelatedWorkElementReference.Count > 0))
                    throw new NotSupportedException("Related work element references not supported during job completion.");
                if (request.JobStatusUpdates.Any(jsu => jsu.AdditionalWork != null))
                    throw new NotSupportedException("Additional work not supported during job completion.");
            }
            if (request.FollowOnWorkOrderReference != null && request.FollowOnWorkOrderReference.Count > 0)
                throw new NotSupportedException("Follow on work order reference not supported during job completion.");
        }
    }
}
