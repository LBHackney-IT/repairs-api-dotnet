using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;

namespace RepairsApi.V2.UseCase
{
    public class CompleteWorkOrderUseCase : ICompleteWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IWorkOrderCompletionGateway _workOrderCompletionGateway;
        private readonly ITransactionManager _transactionManager;

        public CompleteWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IWorkOrderCompletionGateway workOrderCompletionGateway,
            ITransactionManager transactionManager
        )
        {
            _repairsGateway = repairsGateway;
            _workOrderCompletionGateway = workOrderCompletionGateway;
            _transactionManager = transactionManager;
        }

        public async Task<bool> Execute(WorkOrderComplete workOrderComplete)
        {
            var workOrderId = int.Parse(workOrderComplete.WorkOrderReference.ID);

            if (await _workOrderCompletionGateway.IsWorkOrderCompleted(workOrderId))
            {
                return false;
            }

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (workOrder is null)
            {
                return false;
            }

            ValidateRequest(workOrderComplete);
            await using (var transaction = await _transactionManager.Start())
            {
                await _workOrderCompletionGateway.CreateWorkOrderCompletion(workOrderComplete.ToDb(workOrder, null));
                await UpdateWorkOrderStatus(workOrder.Id, workOrderComplete);
                await transaction.Commit();
            }

            return true;
        }

        private async Task UpdateWorkOrderStatus(int workOrderId, WorkOrderComplete workOrderComplete)
        {
            foreach (var update in workOrderComplete.JobStatusUpdates)
            {
                if (update.TypeCode == Generated.JobStatusUpdateTypeCode._0)
                {
                    switch (update.OtherType)
                    {
                        case CustomJobStatusUpdates.COMPLETED:
                            await _repairsGateway.UpdateWorkOrderStatus(workOrderId, WorkStatusCode.Complete);
                            return;
                        case CustomJobStatusUpdates.CANCELLED:
                            await _repairsGateway.UpdateWorkOrderStatus(workOrderId, WorkStatusCode.Canceled);
                            return;
                    }
                }
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
