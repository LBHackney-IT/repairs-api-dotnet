using System.Threading.Tasks;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class DeleteOperativeUseCase : IDeleteOperativeUseCase
    {
        private readonly IOperativesGateway _operativesGateway;
        public DeleteOperativeUseCase(IOperativesGateway operativesGateway)
        {
            _operativesGateway = operativesGateway;
        }

        public Task ExecuteAsync(string operativePrn)
        {
            return _operativesGateway.ArchiveAsync(operativePrn);
        }
    }
}
