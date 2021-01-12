using Microsoft.Extensions.Logging;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.Infrastructure;
using RepairsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase
{
    public class RaiseRepairUseCase : IRaiseRepairUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ILogger<RaiseRepairUseCase> _logger;

        public RaiseRepairUseCase(IRepairsGateway repairsGateway, ILogger<RaiseRepairUseCase> logger)
        {
            _repairsGateway = repairsGateway;
            _logger = logger;
        }

        public async Task<int> Execute(WorkOrder workOrder)
        {
            var id = await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);
            return id;
        }
    }
}
