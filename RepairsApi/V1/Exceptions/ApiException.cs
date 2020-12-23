using System;
using System.Net;

namespace RepairsApi.V1.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(HttpStatusCode statusCode, string message)
            : this(message)
        {
            this.StatusCode = (int)statusCode;
        }

        public ApiException(string message) : base(message) {}

        public ApiException() {}

        public ApiException(string message, Exception innerException) : base(message, innerException) {}
    }
}
