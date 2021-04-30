using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RepairsApi.V2.Notifications;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Services
{
    public class NotifierTests
    {
        [Test]
        public async Task CallsHandlers()
        {
            ServiceCollection services = new ServiceCollection();
            var testNotifier = new TestNotifier();
            services.AddSingleton<INotificationHandler<TestNotification>>(sp => testNotifier);

            var notifier = new Notifier(services.BuildServiceProvider());
            await notifier.Notify(new TestNotification());

            testNotifier.HasBeenCalled.Should().BeTrue();
        }
    }

    internal class TestNotifier : INotificationHandler<TestNotification>
    {
        public bool HasBeenCalled { get; internal set; } = false;

        public Task Notify(TestNotification data)
        {
            HasBeenCalled = true;
            return Task.CompletedTask;
        }
    }

    internal class TestNotification : INotification
    {
    }
}
