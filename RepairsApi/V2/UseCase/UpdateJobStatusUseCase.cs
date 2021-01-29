using System;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using RepairsApi.V2.UseCase.JobStatusUpdatesUseCases;
using JobStatusUpdate = RepairsApi.V2.Generated.JobStatusUpdate;

namespace RepairsApi.V2.UseCase
{
    public class UpdateJobStatusUseCase : IUpdateJobStatusUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;
        private readonly IMoreSpecificSorUseCase _moreSpecificSorUseCase;

        public UpdateJobStatusUseCase(
            IRepairsGateway repairsGateway,
            IJobStatusUpdateGateway jobStatusUpdateGateway,
            IMoreSpecificSorUseCase moreSpecificSorUseCase
        )
        {
            _repairsGateway = repairsGateway;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
            _moreSpecificSorUseCase = moreSpecificSorUseCase;
        }

        public async Task<bool> Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            if (workOrder is null) return false;

            var workElements = await _repairsGateway.GetWorkElementsForWorkOrder(workOrder);

            ProcessActions(jobStatusUpdate);

            await _jobStatusUpdateGateway.CreateJobStatusUpdate(jobStatusUpdate.ToDb(workElements, workOrder));

            return true;
        }

        private void ProcessActions(JobStatusUpdate jobStatusUpdate)
        {
            switch (jobStatusUpdate.TypeCode)
            {
                case JobStatusUpdateTypeCode._80: // More specific SOR Code
                    _moreSpecificSorUseCase.Execute(jobStatusUpdate);
                    break;
            }
        }
    }
}
