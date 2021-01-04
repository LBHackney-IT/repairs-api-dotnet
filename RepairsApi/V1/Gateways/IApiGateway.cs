using System;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
#nullable enable
    public interface IApiGateway
    {
        Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(Uri url, string apiKey)
            where TResponse : class;
    }
}
