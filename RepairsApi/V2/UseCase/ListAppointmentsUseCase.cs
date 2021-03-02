using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.UseCase.Interfaces;

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

            return (await _appointmentsGateway.ListAppointments(GetContractor(workOrder), fromDate, toDate))
                .GroupBy(a => a.Date)
                .Select(a => new AppointmentDayViewModel
                {
                    Date = a.Key.ToDate(),
                    Slots = a.Select(slot => new AppointmentSlot
                    {
                        Description = slot.Description,
                        End = slot.End.ToTime(),
                        Start = slot.Start.ToTime(),
                        Reference = $"{slot.Id}/{slot.Date:yyyy-MM-dd}"
                    }).OrderBy(slot => slot.Start)
                });
        }

        private static string GetContractor(Infrastructure.WorkOrder workOrder)
        {
            return workOrder.AssignedToPrimary?.ContractorReference;
        }
    }
}
