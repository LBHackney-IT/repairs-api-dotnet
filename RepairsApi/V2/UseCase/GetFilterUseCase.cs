using RepairsApi.V2.Configuration;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class GetFilterUseCase : IGetFilterUseCase
    {
        private readonly IActivatorWrapper<IFilterProvider> _activator;

        public GetFilterUseCase(IActivatorWrapper<IFilterProvider> activator)
        {
            _activator = activator;
        }

        public Task<ModelFilterConfiguration> Execute(string modelName)
        {
            var provider = modelName switch
            {
                FilterConstants.WorkOrder => _activator.CreateInstance<WorkOrderFilterProvider>(),
                _ => throw new ResourceNotFoundException($"No filter configuration set up for {modelName}")
            };

            return provider.GetFilter();
        }
    }
}
