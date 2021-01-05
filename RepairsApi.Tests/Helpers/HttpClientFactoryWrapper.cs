using RepairsApi.Tests.ApiMocking;
using System.Net.Http;

namespace RepairsApi.Tests.Helpers
{
    public class HttpClientFactoryWrapper : IHttpClientFactory
    {
        private IHttpClientFactory _innerFactory;
        private MockHttpMessageHandler _handlerMock;
        private HttpClient _client;

        public HttpClientFactoryWrapper(HttpClient client)
        {
            _client = client;
        }

        public HttpClientFactoryWrapper(IHttpClientFactory innerFactory, MockHttpMessageHandler handlerMock)
        {
            _innerFactory = innerFactory;
            _handlerMock = handlerMock;
        }

        public HttpClient CreateClient(string name)
        {
            if (_client != null)
            {
                return _client;
            }

            HttpClient mockClient = BuildNamedMock(name);

            return mockClient;
        }

        private HttpClient BuildNamedMock(string name)
        {
            var namedClient = _innerFactory.CreateClient(name);
            var mockClient = new HttpClient(_handlerMock);

            mockClient.BaseAddress = namedClient.BaseAddress;
            return mockClient;
        }
    }
}
