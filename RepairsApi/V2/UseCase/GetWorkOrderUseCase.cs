using RepairsApi.V2.Boundary;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.Services;

namespace RepairsApi.V2.UseCase
{
    public class GetWorkOrderUseCase : IGetWorkOrderUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IAppointmentsGateway _appointmentGateway;
        private readonly IOptions<DrsOptions> _drsOptions;
        private readonly IFeatureManager _featureManager;
        private readonly IDrsService _drsService;
        private readonly IScheduleOfRatesGateway _sorGateway;

        public GetWorkOrderUseCase(
            IRepairsGateway repairsGateway,
            IAppointmentsGateway appointmentGateway,
            IOptions<DrsOptions> drsOptions,
            IFeatureManager featureManager,
            IDrsService drsService,
            IScheduleOfRatesGateway sorGateway
        )
        {
            _repairsGateway = repairsGateway;
            _appointmentGateway = appointmentGateway;
            _drsOptions = drsOptions;
            _featureManager = featureManager;
            _drsService = drsService;
            _sorGateway = sorGateway;
        }

        public async Task<WorkOrderResponse> Execute(int id)
        {
            var workOrder = await _repairsGateway.GetWorkOrder(id);

            if (await _featureManager.IsEnabledAsync(FeatureFlags.UpdateOperativesOnWorkOrderGet) &&
                await _featureManager.IsEnabledAsync(FeatureFlags.DRSIntegration) &&
                await workOrder.ContractorUsingDrs(_sorGateway))
            {
                await _drsService.UpdateWorkOrderDetails(id);
            }

            var appointment = await _appointmentGateway.GetAppointment(workOrder.Id);
            var canAssignOperative = await workOrder.CanAssignOperative(_sorGateway);

            var workOrderResponse = workOrder.ToResponse(appointment, _drsOptions.Value.ManagementAddress, canAssignOperative);

            return workOrderResponse;
        }
    }
}
