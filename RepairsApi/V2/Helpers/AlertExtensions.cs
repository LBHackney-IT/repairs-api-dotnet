using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using RepairsApi.V2.Domain;

namespace RepairsApi.V2.Helpers
{
    public static class AlertExtensions
    {
        public static string ToDescriptionString(this IEnumerable<Alert> alerts)
        {
            if (alerts.IsNullOrEmpty()) return "";

            var result = "";

            foreach (var alert in alerts)
            {
                result += $"{Environment.NewLine}{alert.ToString()})";
            }

            return result;
        }
    }
}
