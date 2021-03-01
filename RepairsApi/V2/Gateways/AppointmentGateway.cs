using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.Infrastructure.Hackney;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public async Task Create(string appointmentRef, int workOrderId)
        {
            var refArray = appointmentRef.Split('/', 2);
            var slotId = int.Parse(refArray[0]);
            var slotDate = DateTime.ParseExact(refArray[1], DateConstants.DATEFORMAT, null);

            var appoinment = await _repairsContext.AvailableAppointmentDays
                .Where(a => a.Id == slotId)
                .Select(a =>
                new
                {
                    HasOpenSlots = a.ExistingAppointments.Count < a.AvailableCount
                }).SingleOrDefaultAsync();

            if (appoinment is null) throw new ResourceNotFoundException("No Appointment Exists");
            if (!appoinment.HasOpenSlots) throw new NotSupportedException("Appointment slot over capacity");

            await _repairsContext.Appointments.AddAsync(new Infrastructure.Hackney.Appointment
            {
                WorkOrderId = workOrderId,
                DayId = slotId,
                Date = slotDate
            });

            await _repairsContext.SaveChangesAsync();
        }

        public Task<AppointmentDetails> GetAppointment(int id)
        {
            return _repairsContext.Appointments
                .Where(a => a.WorkOrderId == id)
                .Select(a => new AppointmentDetails
                {
                    Date = a.Date,
                    Description = a.Day.AvailableAppointment.Description,
                    Start = a.Day.AvailableAppointment.StartTime,
                    End = a.Day.AvailableAppointment.EndTime
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppointmentDetails>> ListAppointments(string contractorReference, DateTime from, DateTime to)
        {
            if (to <= from) throw new NotSupportedException("Dates are in the incorrect order");

            var availability = await GetAvailability(contractorReference);
            var groupedAppointments = await GetAppointments(contractorReference, from, to);
            var result = BuildResult(from, to, availability, groupedAppointments);

            return result;
        }

        private static List<AppointmentDetails> BuildResult(DateTime from, DateTime to, List<AvailableAppointmentDay> availability, Dictionary<AppointmentInformation, int> counts)
        {
            var fromDate = from.Date;
            var toDate = to.Date;
            var range = toDate - fromDate;
            List<AppointmentDetails> result = new List<AppointmentDetails>();
            for (int i = 0; i < range.Days + 1; i++)
            {
                DateTime date = fromDate.AddDays(i);
                var newAppointments = availability
                    .Where(aa => aa.Day == date.DayOfWeek)
                    .Where(aa => IsCapacityAvailable(counts, aa, date))
                    .Select<AvailableAppointmentDay, AppointmentDetails>(av => new AppointmentDetails
                    {
                        Date = date,
                        Description = av.AvailableAppointment.Description,
                        Id = av.Id,
                        Start = av.AvailableAppointment.StartTime,
                        End = av.AvailableAppointment.EndTime
                    }).ToList();

                result.AddRange(newAppointments);
            }

            return result;
        }

        private static bool IsCapacityAvailable(Dictionary<AppointmentInformation, int> counts, AvailableAppointmentDay aa, DateTime date)
        {
            if (counts.TryGetValue((date.Date, aa.Id), out int count))
            {
                return count < aa.AvailableCount;
            }

            return 0 < aa.AvailableCount;
        }

        private async Task<Dictionary<AppointmentInformation, int>> GetAppointments(string contractorReference, DateTime from, DateTime to)
        {
            var existingAppointments = await _repairsContext.Appointments
                .Where(a => from < a.Date && a.Date < to)
                .Where(a => a.Day.AvailableAppointment.ContractorReference == contractorReference)
                .GroupBy(ea => new { ea.Date, ea.Day.Id })
                .Select(g => new { g.Key.Date, g.Key.Id, Count = g.Count() })
                .ToDictionaryAsync(g => new AppointmentInformation(g.Date.Date, g.Id), g => g.Count);
            return existingAppointments;
        }

        private async Task<List<AvailableAppointmentDay>> GetAvailability(string contractorReference)
        {
            return await _repairsContext.AvailableAppointmentDays
                                        .Where(a => a.AvailableAppointment.ContractorReference == contractorReference)
                                        .Include(a => a.AvailableAppointment)
                                        .ToListAsync();
        }
    }

    [SuppressMessage("Design", "CA1066:Type {0} should implement IEquatable<T> because it overrides Equals", Justification = "Generated Struct By Vs2019")]
    internal struct AppointmentInformation
    {
        public DateTime Date;
        public int Id;

        public AppointmentInformation(DateTime date, int id)
        {
            Date = date;
            Id = id;
        }

        public override bool Equals(object obj)
        {
            return obj is AppointmentInformation other &&
                   Date == other.Date &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Id);
        }

        public void Deconstruct(out DateTime date, out int id)
        {
            date = Date;
            id = Id;
        }

        public static implicit operator (DateTime Date, int Id)(AppointmentInformation value)
        {
            return (value.Date, value.Id);
        }

        public static implicit operator AppointmentInformation((DateTime Date, int Id) value)
        {
            return new AppointmentInformation(value.Date, value.Id);
        }
    }
}
