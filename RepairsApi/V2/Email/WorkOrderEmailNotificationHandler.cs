using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Notifications;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.Email
{
    public class WorkOrderEmailNotificationHandler :
        INotificationHandler<HighCostWorkOrderCreated>,
        INotificationHandler<WorkOrderApproved>,
        INotificationHandler<WorkOrderRejected>
    {
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<WorkOrderEmailNotificationHandler> _logger;
        private readonly IOptions<EmailOptions> _options;

        private IEmailService EmailService => _lazyGovUkService.Value;
        private readonly Lazy<IEmailService> _lazyGovUkService;

        public WorkOrderEmailNotificationHandler(
            Lazy<IEmailService> lazyEmailService,
            IFeatureManager featureMananger,
            ILogger<WorkOrderEmailNotificationHandler> logger,
            IOptions<EmailOptions> options)
        {
            _lazyGovUkService = lazyEmailService;
            _featureManager = featureMananger;
            _logger = logger;
            _options = options;
        }

        private async Task SendMail<T>(T emailRequest, Action logAction)
            where T : EmailRequest
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.NotifyIntegration))
            {
                return;
            }

            await EmailService.SendMailAsync(emailRequest);

            logAction();
        }

        public Task Notify(HighCostWorkOrderCreated data)
        {
            return SendMail(new HighCostWorkOrderEmail(_options.Value.PendingWorkOrderRecipient, data.WorkOrder.Id),
                () => _logger.LogInformation("Mail Sent for creation of high cost work order {WorkOrderId}", data.WorkOrder.Id));
        }

        public Task Notify(WorkOrderApproved data)
        {
            return SendMail(new WorkApprovedEmail(data.WorkOrder.AgentEmail, data.WorkOrder.Id),
                () => _logger.LogInformation("Mail Sent for approval of high cost work order {WorkOrderId}", data.WorkOrder.Id));
        }

        public Task Notify(WorkOrderRejected data)
        {
            return SendMail(new WorkRejectedEmail(data.WorkOrder.AgentEmail, data.WorkOrder.Id),
                () => _logger.LogInformation("Mail Sent for rejection of high cost work order {WorkOrderId}", data.WorkOrder.Id));
        }
    }
}
