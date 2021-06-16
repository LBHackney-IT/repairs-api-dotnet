using RepairsApi.V2.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    internal class NotificationMock : INotifier
    {
        private readonly List<Type> _calledTypes = new List<Type>();
        private readonly List<object> _notifications = new List<object>();
        private readonly List<object> _handlers = new List<object>();

        public bool HaveHandlersBeenCalled() => _calledTypes.Count > 0;

        public bool HaveHandlersBeenCalled<T>() => _calledTypes.Contains(typeof(T));

        public List<T> GetNotifications<T>() => _notifications.OfType<T>().ToList();

        public T GetLastNotification<T>() => _notifications.OfType<T>().LastOrDefault();

        public List<object> GetNotifications() => _notifications;

        public void AddHandler<T>(Action<T> handler) => _handlers.Add(handler);

        public Task Notify<T>(T notification) where T : INotification
        {
            _calledTypes.Add(typeof(T));
            _notifications.Add(notification);
            _handlers.OfType<Action<T>>().ToList().ForEach(a => a(notification));
            return Task.CompletedTask;
        }
    }
}
