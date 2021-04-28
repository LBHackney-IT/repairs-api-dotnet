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
using RepairsApi.V2.Generated;
using RepairsApi.V2.Notifications;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;

namespace RepairsApi.V2.UseCase
{
    public class CompleteWorkOrderUseCase : ICompleteWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IWorkOrderCompletionGateway _workOrderCompletionGateway;
        private readonly ITransactionManager _transactionManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotifier _notifier;

        public CompleteWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IWorkOrderCompletionGateway workOrderCompletionGateway,
            ITransactionManager transactionManager,
            ICurrentUserService currentUserService,
            INotifier notifier
        )
        {
            _repairsGateway = repairsGateway;
            _workOrderCompletionGateway = workOrderCompletionGateway;
            _transactionManager = transactionManager;
            _currentUserService = currentUserService;
            _notifier = notifier;
        }

        public async Task Execute(WorkOrderComplete workOrderComplete)
        {
            var workOrderId = int.Parse(workOrderComplete.WorkOrderReference.ID);

            if (await _workOrderCompletionGateway.IsWorkOrderCompleted(workOrderId))
            {
                throw new NotSupportedException("Cannot complete a work order twice");
            }

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.VerifyCanComplete();

            ValidateRequest(workOrderComplete);
            await using var transaction = await _transactionManager.Start();
            var b = _currentUserService.HasGroup(UserGroups.ContractManager);
            await UpdateWorkOrderStatus(workOrder, workOrderComplete);
            await _workOrderCompletionGateway.CreateWorkOrderCompletion(workOrderComplete.ToDb(workOrder, null));
            await transaction.Commit();
        }

        private async Task UpdateWorkOrderStatus(WorkOrder workOrder, WorkOrderComplete workOrderComplete)
        {
            await workOrderComplete.JobStatusUpdates.ForEachAsync(async update =>
            {
                switch (update.TypeCode)
                {
                    case Generated.JobStatusUpdateTypeCode._0: //Other
                        await HandleCustomType(workOrder, update);
                        break;
                    case Generated.JobStatusUpdateTypeCode._70: // Denied Access
                        if (!_currentUserService.HasGroup(UserGroups.Contractor) &&
                            !_currentUserService.HasGroup(UserGroups.ContractManager))
                            throw new UnauthorizedAccessException("Not Authorised to close jobs");

                        await _repairsGateway.UpdateWorkOrderStatus(workOrder.Id, WorkStatusCode.NoAccess);
                        break;
                    default: throw new NotSupportedException("Unsupported workorder complete job status update code");
                }
            });
        }

        private async Task HandleCustomType(WorkOrder workOrder, Generated.JobStatusUpdates update)
        {
            switch (update.OtherType)
            {
                case CustomJobStatusUpdates.Completed:
                    if (!_currentUserService.HasGroup(UserGroups.Contractor) &&
                        !_currentUserService.HasGroup(UserGroups.ContractManager))
                        throw new UnauthorizedAccessException("Not Authorised to close jobs");

                    await _repairsGateway.UpdateWorkOrderStatus(workOrder.Id, WorkStatusCode.Complete);
                    await _notifier.Notify(new WorkOrderCompleted(workOrder));
                    break;
                case CustomJobStatusUpdates.Cancelled:
                    if (!_currentUserService.HasGroup(UserGroups.Agent) &&
                        !_currentUserService.HasGroup(UserGroups.ContractManager))
                        throw new UnauthorizedAccessException("Not Authorised to cancel jobs");

                    await _repairsGateway.UpdateWorkOrderStatus(workOrder.Id, WorkStatusCode.Canceled);
                    await _notifier.Notify(new WorkOrderCancelled(workOrder));
                    break;
                default: throw new NotSupportedException("Unsupported workorder complete job status update code");
            }
        }

        private static void ValidateRequest(WorkOrderComplete request)
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
