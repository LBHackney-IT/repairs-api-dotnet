using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public class Notifier : INotifier
    {
        private readonly IServiceProvider _serviceProvider;

        public Notifier(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Notify<T>(T notification) where T : INotification
        {
            var handlers = _serviceProvider.GetRequiredService<IEnumerable<INotificationHandler<T>>>();

            await Task.WhenAll(handlers.Select(h => h.Notify(notification)));
        }
    }

    public interface INotifier
    {
        Task Notify<T>(T notification)
            where T : INotification;
    }
}
