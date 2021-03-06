using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Notifications;
using System;
using System.Threading.Tasks;

namespace RepairsApi.V2.Email
{
    public class VariationEmailNotificationHandler :
        INotificationHandler<HighCostVariationCreated>,
        INotificationHandler<VariationApproved>,
        INotificationHandler<VariationRejected>
    {
        private IEmailService EmailService => _lazyGovUkService.Value;
        private readonly Lazy<IEmailService> _lazyGovUkService;
        private readonly IFeatureManager _featureManager;
        private readonly ILogger<VariationEmailNotificationHandler> _logger;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public VariationEmailNotificationHandler(
            Lazy<IEmailService> lazyEmailService,
            IFeatureManager featureMananger,
            ILogger<VariationEmailNotificationHandler> logger,
            IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _lazyGovUkService = lazyEmailService;
            _featureManager = featureMananger;
            _logger = logger;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        private async Task SendMail<T>(T emailRequest, Action logAction)
            where T : EmailRequest
        {
            if (!await _featureManager.IsEnabledAsync(FeatureFlags.NotifyIntegration))
            {
                return;
            }

            using (_logger.BeginScope(Guid.NewGuid()))
            {
                logAction();
                await EmailService.SendMailAsync(emailRequest);
            }
        }

        public Task Notify(VariationRejected data)
        {
            return SendMail(new VariationRejectedEmail(data.Variation.AuthorEmail, data.Rejection.RelatedWorkOrder.Id),
                () => _logger.LogInformation(Resources.SendingHighCostVariationRejection, data.Rejection.RelatedWorkOrder.Id));
        }

        public async Task Notify(HighCostVariationCreated data)
        {
            var emailAddress = await _scheduleOfRatesGateway.GetContractManagerEmail(data.WorkOrder.AssignedToPrimary.ContractorReference);

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return;
            }

            await SendMail(new HighCostVariationCreatedEmail(emailAddress, data.WorkOrder.Id),
                () => _logger.LogInformation(Resources.SendingHighCostVariationCreation, data.WorkOrder.Id));
        }

        public Task Notify(VariationApproved data)
        {
            return SendMail(new VariationApprovedEmail(data.Variation.AuthorEmail, data.Approval.RelatedWorkOrder.Id),
                () => _logger.LogInformation(Resources.SendingHighCostVariationApproval, data.Approval.RelatedWorkOrder.Id));
        }
    }
}
