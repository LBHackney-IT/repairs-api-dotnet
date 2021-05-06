using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Services
{
    public class GovUKNotifyWrapper : IGovUKNotifyWrapper
    {
        private readonly NotifyOptions _options;
        private readonly ILogger _logger;

        public GovUKNotifyWrapper(IOptions<NotifyOptions> options, ILogger<GovUKNotifyWrapper> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task SendMailAsync(EmailVariables variables)
        {
            var notificationClient = new NotificationClient(_options.ApiKey);

            try
            {
                return notificationClient.SendEmailAsync(_options.EmailAddress, _options.CompletedTemplateId, variables);
            }
            catch (NotifyClientException ex)
            {
                _logger.LogError($"[EmailError] Failed to send Email with error {ex.Message}");
                return Task.CompletedTask;
            }
        }
    }

    public class NotifyOptions
    {
        public string ApiKey { get; set; }
        public string CompletedTemplateId { get; set; }
        public string EmailAddress { get; set; }
    }

    public class WorkOrderCompletedEmailVariables : EmailVariables
    {
        public const string WorkOrderId = "workOrderId";
        public const string WorkOrderComment = "workOrderComment";

        public WorkOrderCompletedEmailVariables(int workOrderId, string comment)
        {
            Set(WorkOrderId, workOrderId);
            Set(WorkOrderComment, comment);
        }
    }

    public class EmailVariables : Dictionary<string, object>
    {
        public void Set(string key, object value) => this[key] = value;
    }

    public interface IGovUKNotifyWrapper
    {
        Task SendMailAsync(EmailVariables variables);
    }
}
