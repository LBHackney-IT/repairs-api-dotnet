using System;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public interface IApiGateway
    {
        Task<TResponse> ExecuteRequest<TResponse>(Uri url);
    }
}
