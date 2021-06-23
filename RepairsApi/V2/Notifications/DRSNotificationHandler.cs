using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Notifications
{
    public class DRSNotificationHandler :
        INotificationHandler<WorkOrderOpened>,
        INotificationHandler<WorkOrderCancelled>,
        INotificationHandler<WorkOrderCompleted>,
        INotificationHandler<WorkOrderNoAccess>
    {
        private readonly IFeatureManager _featureManager;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        private IDrsService DrsService => _lazyDrsService.Value;
        private readonly Lazy<IDrsService> _lazyDrsService;

        public DRSNotificationHandler(
            Lazy<IDrsService> lazyDrsService,
            IFeatureManager featureManager,
            IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _lazyDrsService = lazyDrsService;
            _featureManager = featureManager;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        private async Task<bool> UseDrs(WorkOrder workOder)
        {
            return !await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration) ||
                   !await workOder.ContractorUsingDrs(_scheduleOfRatesGateway);
        }

        public async Task Notify(WorkOrderOpened data)
        {
            if (await UseDrs(data.WorkOrder))
            {
                return;
            }
            var order = await DrsService.CreateOrder(data.WorkOrder);
            data.TokenId = order.theBookings.Single().tokenId;
        }

        public async Task Notify(WorkOrderCancelled data)
        {
            if (await UseDrs(data.WorkOrder))
            {
                return;
            }
            await DrsService.CancelOrder(data.WorkOrder);
        }

        public async Task Notify(WorkOrderCompleted data)
        {
            if (await UseDrs(data.WorkOrder))
            {
                return;
            }
            await DrsService.CompleteOrder(data.WorkOrder);
        }

        public Task Notify(WorkOrderNoAccess data)
        {
            return Notify(new WorkOrderCancelled(data.WorkOrder));
        }
    }
}
