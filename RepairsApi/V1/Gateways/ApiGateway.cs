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

        public async Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(Uri url)
            where TResponse : class
        {
            var result = await _httpClient.GetAsync(url).ConfigureAwait(false);
            TResponse response = default;

            if (result.IsSuccessStatusCode)
            {
                var stringResult = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                response = JsonConvert.DeserializeObject<TResponse>(stringResult);
            }

            return new ApiResponse<TResponse>(result.IsSuccessStatusCode, result.StatusCode, response);
        }
    }
}
