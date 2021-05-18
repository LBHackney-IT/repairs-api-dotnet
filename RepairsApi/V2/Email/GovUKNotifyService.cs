using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;
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

        public GovUKNotifyService(IOptions<NotifyOptions> options, ILogger<GovUKNotifyService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task SendMailAsync<TRequest>(TRequest request)
            where TRequest : EmailRequest
        {
            var notificationClient = new NotificationClient(_options.ApiKey);

            try
            {
                return notificationClient.SendEmailAsync(request.Address, ResolveTemplateId<TRequest>(), request);
            }
            catch (NotifyClientException ex)
            {
                _logger.LogError($"[EmailError] Failed to send Email with error {ex.Message}");
                return Task.CompletedTask;
            }
        }

        private string ResolveTemplateId<TRequest>()
            where TRequest : EmailRequest
        {
            var requestName = typeof(TRequest).Name;

            return _options.TemplateIds[requestName];
        }
    }
}
