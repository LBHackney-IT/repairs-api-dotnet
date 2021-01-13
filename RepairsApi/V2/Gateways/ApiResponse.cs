using System.Net;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public class ApiResponse<T>
        where T : class
    {
        private readonly bool _isSuccess;
        private readonly HttpStatusCode _status;
        private readonly T? _content;

        public bool IsSuccess => _isSuccess;
        public HttpStatusCode Status => _status;
        public T? Content => _content;

        public ApiResponse(bool isSuccess, HttpStatusCode status, T? content)
        {
            _isSuccess = isSuccess;
            _status = status;
            _content = content;
        }
    }
}
