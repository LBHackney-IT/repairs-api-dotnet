using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using RepairsApi.V2.Domain;

namespace RepairsApi.V2.UseCase
{
    public class CreateWorkOrderUseCase : ICreateWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;
        private readonly ILogger<CreateWorkOrderUseCase> _logger;

        public CreateWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IScheduleOfRatesGateway scheduleOfRatesGateway,
            ILogger<CreateWorkOrderUseCase> logger)
        {
            _repairsGateway = repairsGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
            _logger = logger;
        }

        public async Task<int> Execute(WorkOrder workOrder)
        {
            ValidateRequest(workOrder);

            workOrder.DateRaised = DateTime.UtcNow;
            await PopulateRateScheduleItems(workOrder);
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

        private async Task PopulateRateScheduleItems(WorkOrder workOrder)
        {
            await workOrder.WorkElements.ForEachAsync(async element =>
            {
                await element.RateScheduleItem.ForEachAsync(async item =>
                {
                    item.DateCreated = DateTime.UtcNow;
                    item.CodeCost = await GetCost(item.ContractReference, item.CustomCode);
                });
            });
        }

        private async Task<double?> GetCost(string contractReference, string customCode)
        {
            return await _scheduleOfRatesGateway.GetCost(contractReference, customCode);
        }
    }
}
