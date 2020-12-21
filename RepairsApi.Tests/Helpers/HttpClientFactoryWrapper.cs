using System.Net.Http;

namespace RepairsApi.Tests.Helpers
{
    public class HttpClientFactoryWrapper : IHttpClientFactory
    {
        private readonly HttpClient _client;

        public HttpClientFactoryWrapper(HttpClient client)
        {
            _client = client;
        }

        public HttpClient CreateClient(string name)
        {
            return _client;
        }
    }
}
