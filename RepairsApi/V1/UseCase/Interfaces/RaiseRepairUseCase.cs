using Microsoft.Extensions.Logging;
using RepairsApi.V1.Domain.Repair;
using RepairsApi.V1.Gateways;
using RepairsApi.V1.UseCase.Interfaces;
using System;
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

        public async Task<bool> Execute(WorkOrder workOrder)
        {
            await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);
            return true;
        }
    }
}
