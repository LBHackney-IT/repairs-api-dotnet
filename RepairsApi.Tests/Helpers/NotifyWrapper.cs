using Moq;
using Notify.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RepairsApi.Tests.Helpers
{
    public class NotifyWrapper : Mock<IAsyncNotificationClient>
    {
        public EmailRecord LastEmail => SentMails.LastOrDefault();
        public List<EmailRecord> SentMails { get; set; } = new List<EmailRecord>();

        public NotifyWrapper()
        {
            Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string email, string templateId, Dictionary<string, dynamic> props, string clientReference, string replyTo) =>
                {
                    var emailRecord = new EmailRecord(email, templateId, props, clientReference, replyTo);
                    SentMails.Add(emailRecord);
                });
        }
    }

    public class EmailRecord
    {
        public EmailRecord(string email, string templateId, Dictionary<string, dynamic> props, string clientReference, string replyTo)
        {
            Email = email;
            TemplateId = templateId;
            Props = props;
            ClientReference = clientReference;
            ReplyTo = replyTo;
        }

        public string Email { get; }
        public string TemplateId { get; }
        public Dictionary<string, object> Props { get; }
        public string ClientReference { get; }
        public string ReplyTo { get; }
    }
}
