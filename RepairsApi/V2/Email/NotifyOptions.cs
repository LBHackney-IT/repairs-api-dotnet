using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RepairsApi.V2.Email
{
    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "Configuration Binding Formatting")]
    public class NotifyOptions
    {
        public string ApiKey { get; set; }

        public string TemplateIds { get; set; }

        private Dictionary<string, string> _internalIds;

        public Dictionary<string, string> GetTemplateIds()
        {
            if (_internalIds != null) return _internalIds;

            _internalIds = new Dictionary<string, string>();

            foreach (string kvPair in TemplateIds.Split(','))
            {
                var item = kvPair.Split(':');

                _internalIds[item[0]] = item[1];
            }

            return _internalIds;
        }
    }
}
