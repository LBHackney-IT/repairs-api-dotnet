using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class GetOperativeUseCase : IGetOperativeUseCase
    {
        private readonly IOperativesGateway _operativesGateway;

        public GetOperativeUseCase(IOperativesGateway operativesGateway)
        {
            _operativesGateway = operativesGateway;
        }

        public async Task<OperativeResponse> ExecuteAsync(string operativePrn)
        {
            var gatewayResponse = await _operativesGateway.GetAsync(operativePrn);
            return gatewayResponse.ToResponse();
        }
    }
}
