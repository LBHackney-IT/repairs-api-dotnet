using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Domain;
using RepairsApi.V2.MiddleWare;
using RepairsApi.V2.Services;
using RepairsApi.V2.Authorisation;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthorizationService _authorizationService;

        public CreateWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IScheduleOfRatesGateway scheduleOfRatesGateway,
            ILogger<CreateWorkOrderUseCase> logger,
            ICurrentUserService currentUserService,
            IDrsService drsService,
            IFeatureManager featureManager,
            IAuthorizationService authorizationService
            )
        {
            _repairsGateway = repairsGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
            _logger = logger;
            _currentUserService = currentUserService;
            _drsService = drsService;
            _featureManager = featureManager;
            _authorizationService = authorizationService;
        }

        public async Task<CreateOrderResult> Execute(WorkOrder workOrder)
        {
            ValidateRequest(workOrder);
            AttachUserInformation(workOrder);
            workOrder.DateRaised = DateTime.UtcNow;

            await SetStatus(workOrder);

            await PopulateRateScheduleItems(workOrder);
            var id = await _repairsGateway.CreateWorkOrder(workOrder);
            _logger.LogInformation(Resources.CreatedWorkOrder);

            if (await _featureManager.IsEnabledAsync(FeatureFlags.DRSINTEGRATION))
            {
                await _drsService.CreateOrder(workOrder);
            }
            return new CreateOrderResult(id, workOrder.StatusCode, workOrder.GetStatus());
        }

        private async Task SetStatus(WorkOrder workOrder)
        {
            var user = _currentUserService.GetUser();
            var authorised = await _authorizationService.AuthorizeAsync(user, workOrder, "RaiseSpendLimit");
            if (await _featureManager.IsEnabledAsync(FeatureFlags.SPENDLIMITS) && !authorised.Succeeded)
            {
                workOrder.StatusCode = WorkStatusCode.PendingApproval;
            }
            else
            {
                workOrder.StatusCode = WorkStatusCode.Open;
            }
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
