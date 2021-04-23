using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Domain;
using RepairsApi.V2.MiddleWare;
using RepairsApi.V2.Services;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Infrastructure.Extensions;

namespace RepairsApi.V2.UseCase
{
    public class CreateWorkOrderUseCase : ICreateWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;
        private readonly ILogger<CreateWorkOrderUseCase> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDrsService _drsService;
        private readonly IFeatureManager _featureManager;
        private readonly IOptions<DrsOptions> _drsOptions;

        public CreateWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IScheduleOfRatesGateway scheduleOfRatesGateway,
            ILogger<CreateWorkOrderUseCase> logger,
            ICurrentUserService currentUserService,
            IDrsService drsService,
            IFeatureManager featureManager,
            IOptions<DrsOptions> drsOptions
            )
        {
            _repairsGateway = repairsGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
            _logger = logger;
            _currentUserService = currentUserService;
            _drsService = drsService;
            _featureManager = featureManager;
            _drsOptions = drsOptions;
        }

        public async Task<CreateOrderResult> Execute(WorkOrder workOrder)
        {
            ValidateRequest(workOrder);
            AttachUserInformation(workOrder);
            workOrder.DateRaised = DateTime.UtcNow;
            workOrder.StatusCode = WorkStatusCode.Open;

            await PopulateRateScheduleItems(workOrder);
            var id = await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);

            var result = new CreateOrderResult(id, workOrder.StatusCode, workOrder.GetStatus());
            if (await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration) &&
                await _drsService.ContractorUsingDrs(workOrder.AssignedToPrimary.ContractorReference))
            {
                var order = await _drsService.CreateOrder(workOrder);
                result.ExternallyManagedAppointment = true;
                var managementUri = new UriBuilder(_drsOptions.Value.ManagementAddress);
                managementUri.Port = -1;
                managementUri.Query = $"tokenId={order.theBookings.Single().tokenId}";
                result.ExternalAppointmentManagementUrl = managementUri.Uri;
            }

            return result;
        }

        private void AttachUserInformation(WorkOrder workOrder)
        {
            if (_currentUserService.IsUserPresent())
            {
                var user = _currentUserService.GetUser();
                workOrder.AgentName = user.Name();
                workOrder.AgentEmail = user.Email();
            }
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
                    item.CodeCost = await GetCost(workOrder.AssignedToPrimary?.ContractorReference, item.CustomCode);
                    item.Original = true;
                    item.OriginalQuantity = item.Quantity.Amount;
                });
            });
        }

        private async Task<double?> GetCost(string contractorReference, string customCode)
        {
            return await _scheduleOfRatesGateway.GetCost(contractorReference, customCode);
        }
    }
}
