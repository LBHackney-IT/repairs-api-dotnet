using System;
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
            workOrder.DateRaised = DateTime.UtcNow;
            await PopulateRateScheduleItems(workOrder);
            var id = await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);
            return id;
        }

        private async Task PopulateRateScheduleItems(WorkOrder workOrder)
        {
            foreach (var element in workOrder.WorkElements)
            {
                foreach (var item in element.RateScheduleItem)
                {
                    item.DateCreated = DateTime.UtcNow;
                    item.CodeCost = await GetCost(item.CustomCode);
                }
            }
        }

        private async Task<double?> GetCost(string customCode)
        {
            return await _scheduleOfRatesGateway.GetCost(customCode);
        }
    }
}
