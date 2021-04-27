using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Domain;
using RepairsApi.V2.Services;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Infrastructure.Extensions;
using RepairsApi.V2.Notifications;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace RepairsApi.V2.UseCase
{
    public class CreateWorkOrderUseCase : ICreateWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;
        private readonly ILogger<CreateWorkOrderUseCase> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFeatureManager _featureManager;
        private readonly IEnumerable<INotificationHandler<WorkOrderOpened>> _handlers;

        public CreateWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IScheduleOfRatesGateway scheduleOfRatesGateway,
            ILogger<CreateWorkOrderUseCase> logger,
            ICurrentUserService currentUserService,
            IAuthorizationService authorizationService,
            IFeatureManager featureManager,
            IEnumerable<INotificationHandler<WorkOrderOpened>> handlers
            )
        {
            _repairsGateway = repairsGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
            _logger = logger;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
            _featureManager = featureManager;
            _handlers = handlers;
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

            await NotifyHandlers(workOrder);
            return new CreateOrderResult(id, workOrder.StatusCode, workOrder.GetStatus());
        }

        private async Task NotifyHandlers(WorkOrder workOrder)
        {
            if (workOrder.StatusCode != WorkStatusCode.Open)
            {
                return;
            }

            var notification = new WorkOrderOpened(workOrder);
            foreach (var handler in _handlers)
            {
                await handler.Notify(notification);
            }
        }

        private async Task SetStatus(WorkOrder workOrder)
        {
            var user = _currentUserService.GetUser();
            var authorised = await _authorizationService.AuthorizeAsync(user, workOrder, "RaiseSpendLimit");
            if (await _featureManager.IsEnabledAsync(FeatureFlags.SpendLimits) && !authorised.Succeeded)
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
