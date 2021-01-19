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
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public RaiseRepairUseCase(
            IRepairsGateway repairsGateway,
            ILogger<RaiseRepairUseCase> logger,
            IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _repairsGateway = repairsGateway;
            _logger = logger;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task<int> Execute(WorkOrder workOrder)
        {
            await AssignContractors(workOrder);
            workOrder.DateRaised = DateTime.UtcNow;
            var id = await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);
            return id;
        }

        private async Task AssignContractors(WorkOrder workOrder)
        {
            foreach (var element in workOrder.WorkElements)
            {
                foreach (var item in element.RateScheduleItem)
                {
                    item.ContractorReference = await GetContractorReference(item.CustomCode);
                }
            }
        }

        private async Task<string> GetContractorReference(string customCode)
        {
            return await _scheduleOfRatesGateway.GetContractorReference(customCode);
        }
    }
}
