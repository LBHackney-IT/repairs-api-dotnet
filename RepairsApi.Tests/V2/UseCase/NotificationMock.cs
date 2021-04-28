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

        public bool HaveHandlersBeenCalled<T>() => _calledTypes.Contains(typeof(T));

        public List<T> GetNotifications<T>() => _notifications.OfType<T>().ToList();

        public T GetLastNotification<T>() => _notifications.OfType<T>().LastOrDefault();

        public List<object> GetNotifications() => _notifications;

        public Task Notify<T>(T notification) where T : INotification
        {
            _calledTypes.Add(typeof(T));
            _notifications.Add(notification);
            return Task.CompletedTask;
        }
    }
}
