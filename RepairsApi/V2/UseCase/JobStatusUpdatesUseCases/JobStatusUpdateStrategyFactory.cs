using System;
using System.Threading.Tasks;
using RepairsApi.V2.Generated;

namespace RepairsApi.V2.UseCase.JobStatusUpdatesUseCases
{
    public interface IJobStatusUpdateStrategyFactory
    {
        Task ProcessActions(JobStatusUpdate jobStatusUpdate);
    }

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

            await strategy?.Execute(jobStatusUpdate);
        }

    }
}
