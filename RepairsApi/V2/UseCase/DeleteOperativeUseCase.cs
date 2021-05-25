using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class DeleteOperativeUseCase : IDeleteOperativeUseCase
    {
        private readonly IOperativeGateway _operativeGateway;
        public DeleteOperativeUseCase(IOperativeGateway operativeGateway)
        {
            _operativeGateway = operativeGateway;
        }

        public async Task<bool> ExecuteAsync(string operativePrn)
        {
            return await _operativeGateway.ArchiveAsync(operativePrn);
        }
    }
}
