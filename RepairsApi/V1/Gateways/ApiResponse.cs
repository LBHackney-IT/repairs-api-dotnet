using System.Net;

namespace RepairsApi.V1.Gateways
{
    public class ApiResponse<T>
    {
        private readonly bool _isSuccess;
        private readonly HttpStatusCode _status;
        private readonly T _content;

        public bool IsSuccess => _isSuccess;
        public HttpStatusCode Status => _status;
        public T Content => _content;

        public ApiResponse(bool isSuccess, HttpStatusCode status, T content)
        {
            _isSuccess = isSuccess;
            _status = status;
            _content = content;
        }
    }
}
