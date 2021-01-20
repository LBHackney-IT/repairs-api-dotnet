using System;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class RaiseRepairUseCase : IRaiseRepairUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ILogger<RaiseRepairUseCase> _logger;

        public RaiseRepairUseCase(
            IRepairsGateway repairsGateway,
            ILogger<RaiseRepairUseCase> logger)
        {
            _repairsGateway = repairsGateway;
            _logger = logger;
        }

        public async Task<int> Execute(WorkOrder workOrder)
        {
            workOrder.DateRaised = DateTime.UtcNow;
            var id = await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);
            return id;
        }
    }
}
