using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class DRSNotificationHandler : INotificationHandler<WorkOrderCreated>, INotificationHandler<WorkOrderCancelled>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureManager _featureManager;

        public DRSNotificationHandler(IServiceProvider serviceProvider, IFeatureManager featureManager)
        {
            _serviceProvider = serviceProvider;
            _featureManager = featureManager;
        }

        public async Task Notify(WorkOrderCreated data)
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration))
            {
                return;
            }

            // Creating here means we do not need to provide config etc if the feature is not turned on
            // Not ideal but accessing feature flags in startup is not clean
            var drsService = _serviceProvider.GetRequiredService<IDrsService>();
            await drsService.CreateOrder(data.WorkOrder);
        }

        public async Task Notify(WorkOrderCancelled data)
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration))
            {
                return;
            }

            // Creating here means we do not need to provide config etc if the feature is not turned on
            // Not ideal but accessing feature flags in startup is not clean
            var drsService = _serviceProvider.GetRequiredService<IDrsService>();
            await drsService.CancelOrder(data.WorkOrder);
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
