using RepairsApi.V2.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    internal class NotificationMock<T> : List<INotificationHandler<WorkOrderOpened>>
    {
        private readonly NotificationSpy _spy;

        public NotificationMock()
        {
            _spy = new NotificationSpy();
            this.Add(_spy);
        }

        public bool HaveHandlersBeenCalled() => _spy.HasBeenCalled();
    }

    internal class NotificationSpy : INotificationHandler<WorkOrderOpened>
    {
        private bool _called;

        public NotificationSpy() => _called = false;

        public Task Notify(WorkOrderOpened data)
        {
            _called = true;
            return Task.CompletedTask;
        }

        internal bool HasBeenCalled() => _called;
    }
}
