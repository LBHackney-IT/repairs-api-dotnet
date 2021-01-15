using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RepairsApi.V2.Enums;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;
using WorkOrderComplete = RepairsApi.V2.Generated.WorkOrderComplete;

namespace RepairsApi.V2.UseCase
{
    public class CompleteWorkOrderUseCase : ICompleteWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IWorkOrderCompletionGateway _workOrderCompletionGateway;

        public CompleteWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IWorkOrderCompletionGateway workOrderCompletionGateway
        )
        {
            _repairsGateway = repairsGateway;
            _workOrderCompletionGateway = workOrderCompletionGateway;
        }

        public async Task<bool> Execute(WorkOrderComplete request)
        {
            var workOrderId = int.Parse(request.WorkOrderReference.ID);
            var workOrder = _repairsGateway.GetWorkOrder(workOrderId);

            if (workOrder is null)
            {
                return false;
            }

            ValidateRequest(request);

            var workOrderComplete = new Infrastructure.WorkOrderComplete
            {
                WorkOrder = workOrder,
                JobStatusUpdates = request.JobStatusUpdates?.Select(jsu => ToDb(jsu, workOrder)).ToList()
            };
            await _workOrderCompletionGateway.CreateWorkOrderCompletion(workOrderComplete);

            return true;
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


        public static JobStatusUpdate ToDb(Generated.JobStatusUpdates update, WorkOrder workOrder)
        {
            return new JobStatusUpdate
            {
                Comments = update.Comments,
                CustomerFeedback = update.CustomerFeedback?.ToDb(),
                EventTime = update.EventTime,
                OperativesAssigned = update.OperativesAssigned?.Select(oa => oa.ToDb()).ToList(),
                OtherType = update.OtherType,
                TypeCode = update.TypeCode,
                RefinedAppointmentWindow = update.RefinedAppointmentWindow?.ToDb(),
                RelatedWorkOrder = workOrder
            };
        }

    }

}
