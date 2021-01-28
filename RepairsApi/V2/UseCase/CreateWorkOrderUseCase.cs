using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class CreateWorkOrderUseCase : ICreateWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly ILogger<CreateWorkOrderUseCase> _logger;

        public CreateWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            ILogger<CreateWorkOrderUseCase> logger)
        {
            _repairsGateway = repairsGateway;
            _logger = logger;
        }

        public async Task<int> Execute(WorkOrder workOrder)
        {
            ValidateRequest(workOrder);

            workOrder.DateRaised = DateTime.UtcNow;
            var id = await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);
            return id;
        }

        private static void ValidateRequest(WorkOrder workOrder)
        {
            if (workOrder.WorkElements?.SelectMany(we => we.Trade).Select(t => t.CustomCode).ToHashSet().Count > 1)
            {
                throw new NotSupportedException("All work elements must be of the same trade");
            }
        }
    }
}
