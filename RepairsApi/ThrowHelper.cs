using RepairsApi.V2.Exceptions;
using System;

namespace RepairsApi
{
    public static class ThrowHelper
    {
        public static void ThrowNotFound(string message) => throw new ResourceNotFoundException(message);
        public static void ThrowUnsupported(string message) => throw new NotSupportedException(message);
        public static void ThrowUnauthorizedAccessException(string message) => throw new UnauthorizedAccessException(message);
        public static void ThrowUpstreamException(int code, string message) => throw new ApiException(code, message);
    }
}
