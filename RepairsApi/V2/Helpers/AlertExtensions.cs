using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using RepairsApi.V2.Domain;

namespace RepairsApi.V2.Helpers
{
    public static class AlertExtensions
    {
        public static string ToCommentsExtendedString(this IEnumerable<Alert> alerts)
        {
            return alerts.IsNullOrEmpty() ? "" : alerts.Aggregate("", (current, alert) => current + $" - {alert}");
        }
        public static string ToDescriptionString(this IEnumerable<Alert> alerts)
        {
            return alerts.IsNullOrEmpty() ? "" : alerts.Aggregate("", (current, alert) => current + $" - {alert.AlertCode}");
        }
    }
}
