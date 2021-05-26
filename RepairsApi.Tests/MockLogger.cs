using Microsoft.Extensions.Logging;
using System;

namespace RepairsApi.Tests
{
    public class MockLogger<T> : ILogger<T>
    {
        private readonly LogAggregator _logAggregator;

        public MockLogger(LogAggregator logAggregator)
        {
            _logAggregator = logAggregator;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new Disposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logAggregator.Log<T>(formatter(state, exception));
        }
    }

    internal class Disposable : IDisposable
    {
        public void Dispose()
        {

        }
    }
}
