using System.Collections.Generic;

namespace RepairsApi.V2.Email
{
    public class NotifyOptions
    {
        public string ApiKey { get; set; }
        public string WorkOrderPendingEmailAddress { get; set; }
        public Dictionary<string, string> TemplateIds { get; set; }
    }
}
