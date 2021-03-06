using System;
using System.Net;

namespace RepairsApi.V2.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(HttpStatusCode statusCode, string message)
            : this((int) statusCode, message)
        {
        }

        public ApiException(int statusCode, string message)
            : this(message)
        {
            this.StatusCode = (int) statusCode;
        }

        public ApiException(string message) : base(message) { }

        public ApiException() { }

        public ApiException(string message, Exception innerException) : base(message, innerException) { }
    }
}
