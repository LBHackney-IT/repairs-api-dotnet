using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
    public class AppointmentGateway : IAppointmentsGateway
    {
        private readonly RepairsContext _repairsContext;

        public AppointmentGateway(RepairsContext repairsContext)
        {
            _repairsContext = repairsContext;
        }

        public async Task Create(int appointmentId, int workOrderId)
        {
            var appoinment = await _repairsContext.AvailableAppointmentDays
                .Where(a => a.Id == appointmentId)
                .Select(a =>
                new {
                    HasOpenSlots = a.ExistingAppointments.Count < a.AvailableCount
                }).SingleOrDefaultAsync();

            if (appoinment is null) throw new ResourceNotFoundException("No Appointment Exists");
            if (!appoinment.HasOpenSlots) throw new NotSupportedException("Appointment slot over capacity");

            await _repairsContext.Appointments.AddAsync(new Infrastructure.Hackney.Appointment
            {
                WorkOrderId = workOrderId,
                DayId = appointmentId
            });
        }

        public async Task<IEnumerable<AvailableAppointmentDay>> ListAppointments(string contractorReference, DateTime from, DateTime to)
        {
            return await _repairsContext.AvailableAppointmentDays
                .Where(a => from <= a.AvailableAppointment.Date && a.AvailableAppointment.Date <= to)
                .Where(a => a.AvailableAppointment.ContractorReference == contractorReference)
                .Where(a => a.ExistingAppointments.Count < a.AvailableCount)
                .ToListAsync();
        }
    }
}
