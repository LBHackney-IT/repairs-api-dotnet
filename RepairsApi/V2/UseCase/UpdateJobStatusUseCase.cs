using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IJobStatusUpdateStrategyFactory _strategyFactory;

        public UpdateJobStatusUseCase(
            IRepairsGateway repairsGateway,
            IJobStatusUpdateGateway jobStatusUpdateGateway,
            IJobStatusUpdateStrategyFactory strategyFactory
        )
        {
            _repairsGateway = repairsGateway;
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
            _strategyFactory = strategyFactory;
        }

        public async Task Execute(JobStatusUpdate jobStatusUpdate)
        {
            var workOrderId = int.Parse(jobStatusUpdate.RelatedWorkOrderReference.ID);

            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            var jsu = jobStatusUpdate.ToDb(workOrder);

            await _strategyFactory.ProcessActions(jsu);

            await _repairsGateway.SaveChangesAsync();

            await _jobStatusUpdateGateway.CreateJobStatusUpdate(jsu);
        }

    }
}
