using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;
using Notify.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V2.Email
{
    public interface IEmailService
    {
        Task SendMailAsync<TRequest>(TRequest request)
            where TRequest : EmailRequest;
    }

    public class GovUKNotifyService : IEmailService
    {
        private readonly NotifyOptions _options;
        private readonly ILogger _logger;
        private readonly IAsyncNotificationClient _notificationClientAsync;

        public GovUKNotifyService(IOptions<NotifyOptions> options,
            ILogger<GovUKNotifyService> logger,
            IAsyncNotificationClient notificationClientAsync)
        {
            _options = options.Value;
            _logger = logger;
            _notificationClientAsync = notificationClientAsync;
        }

        public async Task SendMailAsync<TRequest>(TRequest request)
            where TRequest : EmailRequest
        {
            try
            {
                var requestName = typeof(TRequest).Name;
                if (_options.TemplateIds.TryGetValue(requestName, out var templateId))
                {
                    await _notificationClientAsync.SendEmailAsync(request.Address, templateId, request);
                    _logger.LogInformation("Successfully Sent Email {requestName}", requestName);
                }
                else
                {
                    _logger.LogError("[EmailError] Could not resolve template for email {requestName}", requestName);
                }
            }
            catch (NotifyClientException ex)
            {
                _logger.LogError(ex, "[EmailError] Failed to send Email with error {ex.Message}", ex.Message);
            }
        }
    }
}
