using System;
using System.Threading.Tasks;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Generated.CustomTypes;
using RepairsApi.V2.Helpers;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{

    public class JobStatusUpdateStrategyFactory : IJobStatusUpdateStrategyFactory
    {
        private readonly IActivatorWrapper<IJobStatusUpdateStrategy> _activator;

        public JobStatusUpdateStrategyFactory(IActivatorWrapper<IJobStatusUpdateStrategy> activator)
        {
            _activator = activator;
        }

        public async Task ProcessActions(JobStatusUpdate jobStatusUpdate)
        {
            IJobStatusUpdateStrategy strategy = jobStatusUpdate.TypeCode switch
            {
                JobStatusUpdateTypeCode._80 => _activator.CreateInstance<MoreSpecificSorUseCase>(),
                JobStatusUpdateTypeCode._10020 => _activator.CreateInstance<ApproveVariationUseCase>(),
                JobStatusUpdateTypeCode._125 => _activator.CreateInstance<RejectVariationUseCase>(),
                JobStatusUpdateTypeCode._10010 => _activator.CreateInstance<ContractorAcknowledgeVariationUseCase>(),
                JobStatusUpdateTypeCode._120 => _activator.CreateInstance<JobIncompleteStrategy>(),
                JobStatusUpdateTypeCode._12020 => _activator.CreateInstance<JobIncompleteNeedMaterialsStrategy>(),
                JobStatusUpdateTypeCode._190 => _activator.CreateInstance<RejectWorkOrderStrategy>(),
                JobStatusUpdateTypeCode._200 => _activator.CreateInstance<ApproveWorkOrderStrategy>(),
                JobStatusUpdateTypeCode._0 => ProcessOtherCode(jobStatusUpdate),
                _ => throw new NotSupportedException($"This type code is not supported: {jobStatusUpdate.TypeCode}"),
            };
            if (strategy != null)
            {
                await strategy.Execute(jobStatusUpdate);
            }
        }

        private IJobStatusUpdateStrategy ProcessOtherCode(JobStatusUpdate jobStatusUpdate)
        {
            return jobStatusUpdate.OtherType switch
            {
                CustomJobStatusUpdates.Resume => _activator.CreateInstance<ResumeJobStrategy>(),
                _ => throw new NotSupportedException($"This type code is not supported: {jobStatusUpdate.TypeCode} - {jobStatusUpdate.OtherType}"),
            };
        }
    }
}
