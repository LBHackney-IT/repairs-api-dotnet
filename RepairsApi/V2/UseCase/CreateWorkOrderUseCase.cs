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
using Microsoft.Extensions.Options;
using RepairsApi.V2.Helpers;

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
        private readonly INotifier _notifier;
        private readonly IOptions<DrsOptions> _drsOptions;

        public CreateWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IScheduleOfRatesGateway scheduleOfRatesGateway,
            ILogger<CreateWorkOrderUseCase> logger,
            ICurrentUserService currentUserService,
            IAuthorizationService authorizationService,
            IFeatureManager featureManager,
            INotifier notifier,
            IOptions<DrsOptions> drsOptions
            )
        {
            _repairsGateway = repairsGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
            _logger = logger;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
            _featureManager = featureManager;
            _notifier = notifier;
            _drsOptions = drsOptions;
        }

        public async Task<CreateOrderResult> Execute(WorkOrder workOrder)
        {
            using (_logger.BeginScope(Guid.NewGuid()))
            {
                ValidateRequest(workOrder);
                AttachUserInformation(workOrder);
                workOrder.DateRaised = DateTime.UtcNow;

                await SetStatus(workOrder);

                await PopulateRateScheduleItems(workOrder);
                var id = await _repairsGateway.CreateWorkOrder(workOrder);
                _logger.LogInformation(Resources.CreatedWorkOrder);

                var notification = await NotifyHandlers(workOrder);
                var result = new CreateOrderResult(id, workOrder.StatusCode, workOrder.GetStatus());

                _logger.LogInformation("Notification sent successfully for work order {workOrderId}", workOrder.Id);

                if (await workOrder.ContractorUsingDrs(_scheduleOfRatesGateway))
                {
                    _logger.LogInformation("Contractor using DRS: {workOrderId}", workOrder.Id);

                    result.ExternallyManagedAppointment = true;
                    var managementUri = new UriBuilder(_drsOptions.Value.ManagementAddress);
                    managementUri.Port = -1;
                    managementUri.Query = $"tokenId={notification.TokenId}";
                    result.ExternalAppointmentManagementUrl = managementUri.Uri;

                    try
                    {
                        await NotifyUpdaters(workOrder);
                    }
                    catch (System.ServiceModel.CommunicationException)
                    {
                        _logger.LogError("Error serializing workorder update {workOrderId}", workOrder.Id);
                    }

                }

                _logger.LogInformation("Successfully created work order {workOrderId}", workOrder.Id);

                return result;
            }
        }

        private async Task<WorkOrderPlannerCommentsUpdated> NotifyUpdaters(WorkOrder workOrder)
        {
            var notification = new WorkOrderPlannerCommentsUpdated(workOrder);
            await _notifier.Notify(notification);

            return notification;
        }

        private async Task<WorkOrderOpened> NotifyHandlers(WorkOrder workOrder)
        {
            if (workOrder.StatusCode != WorkStatusCode.Open)
            {
                await _notifier.Notify(new HighCostWorkOrderCreated(workOrder));

                return null;
            }

            var notification = new WorkOrderOpened(workOrder);
            await _notifier.Notify(notification);

            return notification;
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
            if (!_currentUserService.IsUserPresent()) return;

            var user = _currentUserService.GetUser();
            workOrder.AgentName = user.Name();
            workOrder.AgentEmail = user.Email();
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
