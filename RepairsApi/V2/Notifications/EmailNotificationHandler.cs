using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EmailNotificationHandler> _logger;

        private IGovUKNotifyWrapper NotifyService => _lazyGovUkService.Value;
        private readonly Lazy<IGovUKNotifyWrapper> _lazyGovUkService;

        public EmailNotificationHandler(Lazy<IGovUKNotifyWrapper> lazyGovUkService, IFeatureManager featureMananger, ILogger<EmailNotificationHandler> logger)
        {
            _lazyGovUkService = lazyGovUkService;
            _featureManager = featureMananger;
            _logger = logger;
        }

        public async Task Notify(WorkOrderCompleted data)
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.NotifyIntegration))
            {
                return;
            }

            await NotifyService.SendMailAsync(new WorkOrderCompletedEmailVariables(data.WorkOrder.Id, data.Update.Comments));

            _logger.LogInformation("Mail Sent for creation of work order {WorkOrderId}", data.WorkOrder.Id);
        }
    }
}
