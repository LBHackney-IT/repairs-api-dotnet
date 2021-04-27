using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;

namespace RepairsApi.V2.Notifications
{
    public class DRSNotificationHandler : INotificationHandler<WorkOrderCreated>, INotificationHandler<WorkOrderCancelled>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureManager _featureManager;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        // Creating here means we do not need to provide config etc if the feature is not turned on
        // Not ideal but accessing feature flags in startup is not clean
        private IDrsService DrsService => _serviceProvider.GetRequiredService<IDrsService>();

        public DRSNotificationHandler(
            IServiceProvider serviceProvider,
            IFeatureManager featureManager,
            IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _serviceProvider = serviceProvider;
            _featureManager = featureManager;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task Notify(WorkOrderCreated data)
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

    public class WorkOrderCreated : INotification
    {
        public WorkOrderCreated(WorkOrder workOrder)
        {
            WorkOrder = workOrder;
        }

        public WorkOrder WorkOrder { get; }
    }

    public class WorkOrderCancelled : INotification
    {
        public WorkOrderCancelled(WorkOrder workOrder)
        {
            WorkOrder = workOrder;
        }

        public WorkOrder WorkOrder { get; }
    }
}
