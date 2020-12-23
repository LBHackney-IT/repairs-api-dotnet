using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
#nullable enable
    public class ApiGateway : IApiGateway
    {
        private readonly IHttpClientFactory _clientFactory;

        public ApiGateway(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposal of httpclient from the factory is handled by the factory")]
        public async Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(Uri url)
            where TResponse : class
        {
            var client = _clientFactory.CreateClient();
            TResponse? response = default;
            var result = await client.GetAsync(url);

            if (result.IsSuccessStatusCode)
            {
                var stringResult = await result.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<TResponse>(stringResult);
            }
            return new ApiResponse<TResponse>(result.IsSuccessStatusCode, result.StatusCode, response);
        }
    }
}
