using System;
using System.Threading.Tasks;
using RepairsApi.V2.Generated;
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
            IJobStatusUpdateStrategy strategy = null;
            switch (jobStatusUpdate.TypeCode)
            {
                case JobStatusUpdateTypeCode._80: // More specific SOR Code
                    strategy = _activator.CreateInstance<MoreSpecificSorUseCase>();
                    break;
                case JobStatusUpdateTypeCode._0:
                    break;
                default:
                    throw new NotSupportedException($"This type code is not supported: {jobStatusUpdate.TypeCode}");
            }

            if (strategy != null)
            {
                await strategy.Execute(jobStatusUpdate);
            }
        }

    }
}
