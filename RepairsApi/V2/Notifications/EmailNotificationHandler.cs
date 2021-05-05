using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class EmailNotificationHandler : INotificationHandler<WorkOrderCompleted>
    {
        private readonly IFeatureManager _featureManager;

        private IGovUKNotifyWrapper NotifyService => _lazyGovUkService.Value;
        private readonly Lazy<IGovUKNotifyWrapper> _lazyGovUkService;

        public EmailNotificationHandler(Lazy<IGovUKNotifyWrapper> lazyGovUkService, IFeatureManager featureMananger)
        {
            _lazyGovUkService = lazyGovUkService;
            _featureManager = featureMananger;
        }

        public async Task Notify(WorkOrderCompleted data)
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.NotifyIntegration))
            {
                return;
            }

            await NotifyService.SendMailAsync(new WorkOrderCompletedEmailVariables(data.WorkOrder.Id, data.Update.Comments));
        }
    }
}
