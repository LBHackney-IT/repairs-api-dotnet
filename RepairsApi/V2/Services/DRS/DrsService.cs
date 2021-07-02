using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core.Exceptions;
using Castle.Core.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Services.DRS;
using V2_Generated_DRS;
using WorkElement = RepairsApi.V2.Infrastructure.WorkElement;

namespace RepairsApi.V2.Services
{

    public class DrsService : IDrsService
    {
        private readonly SOAP _drsSoap;
        private readonly IOptions<DrsOptions> _drsOptions;
        private readonly ILogger<DrsService> _logger;
        private readonly IDrsMapping _drsMapping;
        private readonly IOperativesGateway _operativesGateway;
        private readonly IAppointmentsGateway _appointmentsGateway;
        private string _sessionId;

        public DrsService(
            SOAP drsSoap,
            IOptions<DrsOptions> drsOptions,
            ILogger<DrsService> logger,
            IDrsMapping drsMapping,
            IOperativesGateway operativesGateway,
            IAppointmentsGateway appointmentsGateway)
        {
            _drsSoap = drsSoap;
            _drsOptions = drsOptions;
            _logger = logger;
            _drsMapping = drsMapping;
            _operativesGateway = operativesGateway;
            _appointmentsGateway = appointmentsGateway;
        }

        public async Task OpenSession()
        {
            var xmbOpenSession = new xmbOpenSession
            {
                login = _drsOptions.Value.Login,
                password = _drsOptions.Value.Password
            };
            var response = await _drsSoap.openSessionAsync(new openSession
            {
                openSession1 = xmbOpenSession
            });
            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int) response.@return.status, response.@return.errorMsg);
            }
            _sessionId = response.@return.sessionId;
        }

        public async Task<order> CreateOrder(WorkOrder workOrder)
        {
            using var scope = _logger.BeginScope(Guid.NewGuid());

            await CheckSession();

            var createOrder = await _drsMapping.BuildCreateOrderRequest(_sessionId, workOrder);

            _logger.LogInformation("DRS Order Creating for Work order {WorkOrderId} {request}", workOrder.Id, createOrder);

            var response = await _drsSoap.createOrderAsync(createOrder);

            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int) response.@return.status, response.@return.errorMsg);
            }

            return response.@return.theOrder;
        }

        public async Task CancelOrder(WorkOrder workOrder)
        {
            using var scope = _logger.BeginScope(Guid.NewGuid());

            await CheckSession();

            var deleteOrder = await _drsMapping.BuildDeleteOrderRequest(_sessionId, workOrder);

            _logger.LogInformation("DRS Order Deleting for Work order {WorkOrderId} {request}", workOrder.Id, deleteOrder);

            var response = await _drsSoap.deleteOrderAsync(deleteOrder);
            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int) response.@return.status, response.@return.errorMsg);
            }

            _logger.LogInformation("DRS Order Deleted for Work order {WorkOrderId}", workOrder.Id);
        }

        public async Task CompleteOrder(WorkOrder workOrder)
        {
            using var scope = _logger.BeginScope(Guid.NewGuid());

            await CheckSession();

            var drsOrder = await SelectOrder(workOrder.Id);

            if (!drsOrder.IsScheduled())
            {
                _logger.LogError("Cannot complete work order ({WorkOrderId}) as it has not been scheduled in DRS, cancelling instead", workOrder.Id);
                await CancelOrder(workOrder);
                return;
            }

            await CompleteBooking(workOrder, drsOrder);

            _logger.LogInformation("DRS completed work order {WorkOrderId}", workOrder.Id);
        }

        private async Task CompleteBooking(WorkOrder workOrder, order drsOrder)
        {
            var updateBooking = await _drsMapping.BuildCompleteOrderUpdateBookingRequest(_sessionId, workOrder, drsOrder);

            _logger.LogInformation("DRS completing booking for work order {WorkOrderId} {request}", workOrder.Id, updateBooking);

            var response = await _drsSoap.updateBookingAsync(updateBooking);
            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int) response.@return.status, response.@return.errorMsg);
            }

            _logger.LogInformation("DRS completed booking for work order {WorkOrderId}", workOrder.Id);
        }

        public async Task<order> SelectOrder(int workOrderId)
        {
            using var scope = _logger.BeginScope(Guid.NewGuid());

            await CheckSession();

            var selectOrder = new selectOrder
            {
                selectOrder1 = new xmbSelectOrder
                {
                    sessionId = _sessionId,
                    primaryOrderNumber = new[]
                    {
                        workOrderId.ToString()
                    }
                }
            };

            _logger.LogInformation("DRS selecting order {WorkOrderId} {request}", workOrderId, selectOrder);

            var selectOrderResponse = await _drsSoap.selectOrderAsync(selectOrder);
            if (selectOrderResponse.@return.status != responseStatus.success)
            {
                _logger.LogError(selectOrderResponse.@return.errorMsg);
                throw new ApiException((int) selectOrderResponse.@return.status, selectOrderResponse.@return.errorMsg);
            }
            var drsOrder = selectOrderResponse.@return.theOrders.First();
            return drsOrder;
        }

        public async Task UpdateWorkOrderDetails(int workOrderId)
        {
            var order = await SelectOrder(workOrderId);

            var theBooking = order.theBookings.First();
            var theResources = theBooking.theResources;

            if (theResources.IsNullOrEmpty()) return;

            var operativePayrollIds = theResources.Select(r => r.externalResourceCode);
            var operatives = await Task.WhenAll(operativePayrollIds.Select(i => _operativesGateway.GetAsync(i)));

            await _operativesGateway.AssignOperatives(workOrderId, operatives.Select(o => o.Id).ToArray());
            await _appointmentsGateway.SetTimedBooking(
                workOrderId,
                theBooking.planningWindowStart,
                theBooking.planningWindowEnd
            );
        }

        private async Task CheckSession()
        {
            if (_sessionId is null)
            {
                await OpenSession();
            }
        }
    }
}
