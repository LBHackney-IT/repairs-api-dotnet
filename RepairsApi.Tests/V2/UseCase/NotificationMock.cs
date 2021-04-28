using RepairsApi.V2.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.UseCase
{
    internal class NotificationMock : INotifier
    {
        private bool _hasBeenCalled = false;

        public bool HaveHandlersBeenCalled() => _hasBeenCalled;

        public Task Notify<T>(T notification) where T : INotification
        {
            _hasBeenCalled = true;
            return Task.CompletedTask;
        }
    }
}
