using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class DRSNotificationHandler : INotificationHandler<WorkOrderOpened>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureManager _featureManager;

        public DRSNotificationHandler(IServiceProvider serviceProvider, IFeatureManager featureManager)
        {
            _serviceProvider = serviceProvider;
            _featureManager = featureManager;
        }

        public async Task Notify(WorkOrderOpened data)
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration))
            {
                return;
            }

            // Creating here means we do not need to provde config etc if the feature is not turned on
            // Not ideal but accessing feature flags in startup is not clean
            var drsService = _serviceProvider.GetRequiredService<IDrsService>();
            await drsService.CreateOrder(data.WorkOrder);
        }
    }

    public class WorkOrderOpened : INotification
    {
        public WorkOrderOpened(WorkOrder workOrder)
        {
            WorkOrder = workOrder;
        }

        public WorkOrder WorkOrder { get; }
    }
}
