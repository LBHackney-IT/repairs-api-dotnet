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
            /*
            https://www.oscre.org/idm?content=entity/JobStatusUpdateTypeCode
            */
            IJobStatusUpdateStrategy strategy = jobStatusUpdate.TypeCode switch
            {
                // More specific SOR Code - (means variation) 
                JobStatusUpdateTypeCode._80 => _activator.CreateInstance<MoreSpecificSorUseCase>(),

                //Variation approved 
                JobStatusUpdateTypeCode._10020 => _activator.CreateInstance<ApproveVariationUseCase>(),

                //Variation Rejected
                JobStatusUpdateTypeCode._125 => _activator.CreateInstance<RejectVariationUseCase>(),

                //Variation acknowledged by contractor, workorder set to in progress
                JobStatusUpdateTypeCode._10010 => _activator.CreateInstance<ContractorAcknowledgeVariationUseCase>(),

                // Job Incomplete
                JobStatusUpdateTypeCode._120 => _activator.CreateInstance<JobIncompleteStrategy>(),

                // Job incomplete - need materials
                JobStatusUpdateTypeCode._12020 => _activator.CreateInstance<JobIncompleteNeedMaterialsStrategy>(),
                // Other
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
                _ => null,
            };
        }
    }
}
