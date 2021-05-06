using RepairsApi.V2.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.Tests.Helpers
{
    public class MockGovUKNotifyWrapper : IGovUKNotifyWrapper
    {
        public List<EmailVariables> SentMails { get; private set; } = new List<EmailVariables>();

        public Task SendMailAsync(EmailVariables variables)
        {
            SentMails.Add(variables);
            return Task.CompletedTask;
        }
    }
}
