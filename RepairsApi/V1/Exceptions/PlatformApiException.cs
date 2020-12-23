using System.Net;

namespace RepairsApi.V1.Exceptions
{
    public class PlatformApiException : ResourceAcquisitionException
    {
        private readonly HttpStatusCode _httpStatusCode;

        public override int StatusCode => (int)_httpStatusCode;

        public PlatformApiException(HttpStatusCode httpStatusCode)
            : base($"Platform Api Failed with code {httpStatusCode}" )
        {
            _httpStatusCode = httpStatusCode;
        }

    }
}
