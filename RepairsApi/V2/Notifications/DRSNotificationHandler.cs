using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;

namespace RepairsApi.V2.Notifications
{
    public class DRSNotificationHandler : INotificationHandler<WorkOrderOpened>, INotificationHandler<WorkOrderCancelled>
    {
        private readonly IFeatureManager _featureManager;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        private IDrsService DrsService => _lazyDrsService.Value;
        private readonly Lazy<IDrsService> _lazyDrsService;

        public DRSNotificationHandler(
            IServiceProvider serviceProvider,
            IFeatureManager featureManager,
            IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _featureManager = featureManager;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;

            // Creating using a Lazy<T> means we do not need to provide config etc if the feature is not turned on
            // Not ideal but accessing feature flags in startup is not clean
            _lazyDrsService = new Lazy<IDrsService>(serviceProvider.GetRequiredService<IDrsService>);
        }

        public async Task Notify(WorkOrderOpened data)
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration) ||
                !await ContractorUsingDrs(data.WorkOrder.AssignedToPrimary.ContractorReference))
            {
                return;
            }
            await DrsService.CreateOrder(data.WorkOrder);
        }

        public async Task Notify(WorkOrderCancelled data)
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration) ||
                !await ContractorUsingDrs(data.WorkOrder.AssignedToPrimary.ContractorReference))
            {
                return;
            }
            await DrsService.CancelOrder(data.WorkOrder);
        }

        private async Task<bool> ContractorUsingDrs(string contractorRef)
        {
            var contractor = await _scheduleOfRatesGateway.GetContractor(contractorRef);
            return contractor.UseExternalScheduleManager;
        }
    }
}
