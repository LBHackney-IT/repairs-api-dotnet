using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
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
        private string _sessionId;

        public DrsService(
            V2_Generated_DRS.SOAP drsSoap,
            IOptions<DrsOptions> drsOptions,
            ILogger<DrsService> logger,
            IDrsMapping drsMapping
        )
        {
            _drsSoap = drsSoap;
            _drsOptions = drsOptions;
            _logger = logger;
            _drsMapping = drsMapping;

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
            await CheckSession();

            var createOrder = await _drsMapping.BuildCreateOrderRequest(_sessionId, workOrder);
            var response = await _drsSoap.createOrderAsync(createOrder);
            if (response.@return.status == responseStatus.success) return response.@return.theOrder;

            _logger.LogError(response.@return.errorMsg);
            throw new ApiException((int) response.@return.status, response.@return.errorMsg);

        }

        public async Task CancelOrder(WorkOrder workOrder)
        {
            await CheckSession();

            var deleteOrder = await _drsMapping.BuildDeleteOrderRequest(_sessionId, workOrder);
            var response = await _drsSoap.deleteOrderAsync(deleteOrder);
            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int) response.@return.status, response.@return.errorMsg);
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