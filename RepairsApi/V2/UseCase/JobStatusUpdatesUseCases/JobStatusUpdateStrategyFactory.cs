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
                // More specific SOR Code - (probably means variation) Approval is 100 - JobstatustypeCode - 100,
                //Reason code = Approved = 70, Workstatuscode = 80
                JobStatusUpdateTypeCode._80 => _activator.CreateInstance<MoreSpecificSorUseCase>(),
                //Variation approved
                JobStatusUpdateTypeCode._10020 => _activator.CreateInstance<MoreSpecificSorUseCase>(),
                //Variation acknowlwdged by contractor
                JobStatusUpdateTypeCode._10010 => _activator.CreateInstance<MoreSpecificSorUseCase>(),
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
                CustomJobStatusUpdates.RESUME => _activator.CreateInstance<ResumeJobStrategy>(),
                _ => null,
            };
        }
    }
}
