using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Infrastructure;
using V2_Generated_DRS;

namespace RepairsApi.V2.Services
{
    public class DrsService : IDrsService
    {
        private readonly SOAP _drsSoap;
        private readonly IOptions<DrsOptions> _drsOptions;
        private readonly ILogger<DrsService> _logger;
        private string _sessionId;

        public DrsService(
            V2_Generated_DRS.SOAP drsSoap,
            IOptions<DrsOptions> drsOptions,
            ILogger<DrsService> logger
        )
        {
            _drsSoap = drsSoap;
            _drsOptions = drsOptions;
            _logger = logger;

        }

        public async Task OpenSession()
        {
            var xmbOpenSession = new xmbOpenSession
            {
                login = _drsOptions.Value.Login,
                password = _drsOptions.Value.Password
            };
            var response = await _drsSoap.openSessionAsync(new openSession { openSession1 = xmbOpenSession });
            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int)response.@return.status, response.@return.errorMsg);
            }
            _sessionId = response.@return.sessionId;
        }

        public async Task CreateOrder(WorkOrder workOrder)
        {
            await CheckSession();
            var createOrder = new createOrder
            {
                createOrder1 = new xmbCreateOrder
                {
                    sessionId = _sessionId,
                    theOrder = new order
                    {
                        status = orderStatus.PLANNED,
                        primaryOrderNumber = workOrder.Id.ToString(),
                        orderComments = "Work Order Created",
                        contract = workOrder.AssignedToPrimary.ContractorReference,
                        locationID = workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference,
                        priority = workOrder.WorkPriority.PriorityDescription,
                        targetDate = workOrder.WorkPriority.RequiredCompletionDateTime ?? DateTime.UtcNow,
                        userId = workOrder.AgentEmail ?? workOrder.AgentName,
                        theLocation = new location
                        {
                            locationId = workOrder.Site.PropertyClass.FirstOrDefault()?.PropertyReference,
                            name = workOrder.Site.Name,
                            address1 = workOrder.Site.PropertyClass.FirstOrDefault()?.Address.AddressLine,
                            postCode = workOrder.Site.PropertyClass.FirstOrDefault()?.Address.PostalCode,
                            contract = workOrder.AssignedToPrimary.ContractorReference
                        }
                    }
                }
            };
            var response = await _drsSoap.createOrderAsync(createOrder);
            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int)response.@return.status, response.@return.errorMsg);
            }
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
