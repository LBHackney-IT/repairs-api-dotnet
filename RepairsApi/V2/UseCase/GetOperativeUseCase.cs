using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class GetOperativeUseCase : IGetOperativeUseCase
    {
        private readonly IOperativeGateway _operativeGateway;

        public GetOperativeUseCase(IOperativeGateway operativeGateway)
        {
            _operativeGateway = operativeGateway;
        }

        public async Task<OperativeResponse> ExecuteAsync(string operativePrn)
        {
            var gatewayResponse = await _operativeGateway.GetByPayrollNumberAsync(operativePrn);
            return gatewayResponse.ToResponse();
        }
    }
}
