using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public class ApiGateway : IApiGateway
    {
        private readonly HttpClient _httpClient;

        public ApiGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> ExecuteRequest<TResponse>(Uri url)
            where TResponse : class
        {
            var result = await _httpClient.GetAsync(url).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();

            if (result.Content == null) return null;

            var stringResult = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TResponse>(stringResult);
        }
    }
}
