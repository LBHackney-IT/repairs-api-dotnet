using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class ListAppointmentsUseCase : IListAppointmentsUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IAppointmentsGateway _appointmentsGateway;

        public ListAppointmentsUseCase(IRepairsGateway repairsGateway, IAppointmentsGateway appointmentsGateway)
        {
            _repairsGateway = repairsGateway;
            _appointmentsGateway = appointmentsGateway;
        }

        public async Task<IEnumerable<AppointmentDayViewModel>> Execute(int workOrderId, DateTime fromDate, DateTime toDate)
        {
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);

            if (workOrder is null) throw new ResourceNotFoundException("Work Order does not exist");

            return (await _appointmentsGateway.ListAppointments(workOrderId.ToString(), fromDate, toDate))
                .Select(a => new AppointmentDayViewModel
            {
            });
        }
    }
}
