using System;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.V2.UseCase
{
    public class UpdateJobStatusUseCase : IUpdateJobStatusUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;

        public UpdateJobStatusUseCase(
            IRepairsGateway repairsGateway,
            IJobStatusUpdateGateway jobStatusUpdateGateway
        )
        {
            _repairsGateway = repairsGateway;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
        }

        public async Task<bool> Execute(JobStatusUpdate request)
        {
            var workOrderId = int.Parse(request.RelatedWorkOrderReference.ID);

            var workOrder = _repairsGateway.GetWorkOrder(workOrderId);
            if (workOrder is null) return false;

            var workElements = _repairsGateway.GetWorkElementsForWorkOrder(workOrder);

            await _jobStatusUpdateGateway.CreateJobStatusUpdate(
                new Infrastructure.JobStatusUpdate
                {
                    RelatedWorkElement = workElements.ToList(),
                    EventTime = DateTime.Now,
                    TypeCode = request.TypeCode,
                    AdditionalWork = request.AdditionalWork?.ToDb(),
                    Comments = request.Comments,
                    CustomerCommunicationChannelAttempted = request.CustomerCommunicationChannelAttempted?.ToDb(),
                    CustomerFeedback = request.CustomerFeedback?.ToDb(),
                    MoreSpecificSORCode = request.MoreSpecificSORCode?.ToDb(),
                    OperativesAssigned = request.OperativesAssigned?.Select(oa => oa.ToDb()).ToList(),
                    OtherType = request.OtherType,
                    RefinedAppointmentWindow = request.RefinedAppointmentWindow?.ToDb(),
                    RelatedWorkOrder = workOrder
                }
            );

            return true;
        }
    }
}
