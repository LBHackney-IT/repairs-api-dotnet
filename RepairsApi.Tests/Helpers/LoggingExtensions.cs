using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace RepairsApi.Tests.Helpers
{
    public static class LoggingExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> mockLogger, LogLevel level)
        {
            mockLogger.Verify(x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))
            );
        }
    }
}
