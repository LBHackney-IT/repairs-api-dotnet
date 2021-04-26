using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
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
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;
        private string _sessionId;

        public DrsService(
            V2_Generated_DRS.SOAP drsSoap,
            IOptions<DrsOptions> drsOptions,
            ILogger<DrsService> logger,
            IDrsMapping drsMapping,
            IScheduleOfRatesGateway scheduleOfRatesGateway
        )
        {
            _drsSoap = drsSoap;
            _drsOptions = drsOptions;
            _logger = logger;
            _drsMapping = drsMapping;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;

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

        public async Task CreateOrder(WorkOrder workOrder)
        {
            if (!await ContractorUsingDrs(workOrder.AssignedToPrimary.ContractorReference)) return;
            await CheckSession();

            var createOrder = await _drsMapping.BuildCreateOrderRequest(_sessionId, workOrder);
            var response = await _drsSoap.createOrderAsync(createOrder);
            if (response.@return.status != responseStatus.success)
            {
                _logger.LogError(response.@return.errorMsg);
                throw new ApiException((int) response.@return.status, response.@return.errorMsg);
            }
        }

        public async Task<bool> ContractorUsingDrs(string contractorRef)
        {
            var contractor = await _scheduleOfRatesGateway.GetContractor(contractorRef);
            return contractor.UseExternalScheduleManager;
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
