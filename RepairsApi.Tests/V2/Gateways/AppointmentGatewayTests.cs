using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2;
using RepairsApi.V2.Infrastructure;
using Appointment = RepairsApi.V2.Infrastructure.Hackney.Appointment;

namespace RepairsApi.Tests.V2.Gateways
{
    public class AppointmentGatewayTests
    {
        private AppointmentGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new AppointmentGateway(InMemoryDb.Instance);
        }

        [Test]
        public async Task ListForToday()
        {
            SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 5),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            items.Should().HaveCount(2);

            items.Count(item => item.Description == "AM").Should().Be(1);
            items.Count(item => item.Description == "PM").Should().Be(1);
        }

        [Test]
        public async Task OnlyIncludeRelevantDays()
        {
            SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 5), new DaySeedModel(DateTime.UtcNow.AddDays(1).DayOfWeek, 5),
                new DaySeedModel(DateTime.UtcNow.AddDays(2).DayOfWeek, 5),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            items.Should().HaveCount(2);
            items.Count(item => item.Description == "AM").Should().Be(1);
            items.Count(item => item.Description == "PM").Should().Be(1);
        }

        [Test]
        public async Task EmptyListForNoAppointments()
        {
            SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 0),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            items.Should().BeEmpty();
        }

        [Test]
        public async Task EmptyListForOtherContractor()
        {
            SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 5),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12)),
                new AppointmentSeedModel("PM", new DateTime().AddHours(13), new DateTime().AddHours(18)),
            });

            var items = await _classUnderTest.ListAppointments("otherContractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            items.Should().BeEmpty();
        }

        [Test]
        public async Task ThrowsIfDatesAreWrongWayRound()
        {
            Func<Task> testFunc = async () => await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(8), DateTime.UtcNow.AddDays(1));

            await testFunc.Should().ThrowAsync<NotSupportedException>();
        }


        [Test]
        public async Task BookSlotAppointment()
        {
            var startTime = new DateTime().AddHours(8);
            var endTime = new DateTime().AddHours(12);
            var ids = SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 1),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel("AM", startTime, endTime)
            });

            var preBookItems = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
            var appointmentRef = GenerateAppointmentRef(ids.First(), DateTime.UtcNow);
            var workOrderId = 100001;
            await _classUnderTest.CreateSlotBooking(appointmentRef, workOrderId);
            var postBookItems = await _classUnderTest.ListAppointments("contractor", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var appointment = await InMemoryDb.Instance.Appointments.SingleOrDefaultAsync(a => a.WorkOrderId == workOrderId);
            appointment.Should().NotBeNull();
            appointment.StartTime.Should().Be(startTime);
            appointment.EndTime.Should().Be(endTime);

            preBookItems.Should().HaveCount(1);
            postBookItems.Should().HaveCount(0);
        }

        [Test]
        public async Task UpdateSlotAppointment()
        {
            const int workOrderId = 100001;
            var startTime = new DateTime().AddHours(8);
            var endTime = new DateTime().AddHours(12);
            var ids = SeedAppointmentData("contractor", new[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 1),
            }, new[]
            {
                new AppointmentSeedModel("AM", startTime, endTime)
            });
            var existingAppointment = new Appointment
            {
                Date = DateTime.UtcNow,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(5),
                WorkOrderId = workOrderId
            };
            await InMemoryDb.Instance.Appointments.AddAsync(existingAppointment);
            await InMemoryDb.Instance.SaveChangesAsync();

            var appointmentRef = GenerateAppointmentRef(ids.First(), DateTime.UtcNow);
            await _classUnderTest.CreateSlotBooking(appointmentRef, workOrderId);

            var appointment = await InMemoryDb.Instance.Appointments.SingleOrDefaultAsync(a => a.WorkOrderId == workOrderId);
            appointment.Should().NotBeNull();
            appointment.StartTime.Should().Be(startTime);
            appointment.EndTime.Should().Be(endTime);
        }

        [Test]
        public async Task BookTimedAppointment()
        {
            const int workOrderId = 100001;
            await CreateWorkOrder(workOrderId);
            var startTime = DateTime.UtcNow;
            var endTime = DateTime.UtcNow.AddHours(5);

            await _classUnderTest.SetTimedBooking(workOrderId, startTime, endTime);

            var appointment = await InMemoryDb.Instance.Appointments.SingleOrDefaultAsync(a => a.WorkOrderId == workOrderId);
            appointment.Should().NotBeNull();
            appointment.WorkOrderId.Should().Be(workOrderId);
            appointment.Date.Should().Be(startTime.Date);
            appointment.StartTime.Should().Be(startTime);
            appointment.EndTime.Should().Be(endTime);
        }

        [Test]
        public async Task UpdateTimedAppointment()
        {
            const int workOrderId = 100001;
            await CreateWorkOrder(workOrderId);
            var existingAppointment = new Appointment
            {
                Date = DateTime.UtcNow,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(5),
                WorkOrderId = workOrderId
            };
            await InMemoryDb.Instance.Appointments.AddAsync(existingAppointment);
            await InMemoryDb.Instance.SaveChangesAsync();

            var startTime = existingAppointment.StartTime.AddDays(7);
            var endTime = existingAppointment.EndTime.AddDays(7);

            await _classUnderTest.SetTimedBooking(workOrderId, startTime, endTime);

            var appointment = await InMemoryDb.Instance.Appointments.SingleOrDefaultAsync(a => a.WorkOrderId == workOrderId);
            appointment.Should().NotBeNull();
            appointment.WorkOrderId.Should().Be(workOrderId);
            appointment.Date.Should().Be(startTime.Date);
            appointment.StartTime.Should().Be(startTime);
            appointment.EndTime.Should().Be(endTime);
        }

        private static async Task CreateWorkOrder(int workOrderId)
        {
            await InMemoryDb.Instance.WorkOrders.AddAsync(new WorkOrder
            {
                Id = workOrderId
            });

            await InMemoryDb.Instance.SaveChangesAsync();
        }

        [Test]
        public async Task ThrowsIfWorkOrderNotFound()
        {
            const int workOrderId = 1234;
            var startTime = DateTime.UtcNow;
            var endTime = DateTime.UtcNow.AddHours(5);

            Func<Task> act = () => _classUnderTest.SetTimedBooking(workOrderId, startTime, endTime);

            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage(Resources.WorkOrderNotFound);
        }

        private static string GenerateAppointmentRef(int id, DateTime date)
        {
            return $"{id}/{date:yyyy-MM-dd}";
        }

        [Test]
        public async Task ThrowsWhenOverBooking()
        {
            var ids = SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 1),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12))
            });
            await _classUnderTest.CreateSlotBooking(GenerateAppointmentRef(ids.First(), DateTime.UtcNow), 100001);

            Func<Task> testFunc = async () => await _classUnderTest.CreateSlotBooking(GenerateAppointmentRef(ids.First(), DateTime.UtcNow), 100001);

            await testFunc.Should().ThrowAsync<NotSupportedException>();
        }


        [Test]
        public async Task AllowsBookingsForDifferentDates()
        {
            var ids = SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 1),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel("AM", new DateTime().AddHours(8), new DateTime().AddHours(12))
            });
            await _classUnderTest.CreateSlotBooking(GenerateAppointmentRef(ids.First(), DateTime.UtcNow), 100001);

            Func<Task> testFunc = async () => await _classUnderTest.CreateSlotBooking(GenerateAppointmentRef(ids.First(), DateTime.UtcNow), 100001);
            Func<Task> nextWeekTestFunc = async () => await _classUnderTest.CreateSlotBooking(GenerateAppointmentRef(ids.First(), DateTime.UtcNow.AddDays(7)), 100001);

            await testFunc.Should().ThrowAsync<NotSupportedException>();
            await nextWeekTestFunc.Should().NotThrowAsync<NotSupportedException>();
        }


        [Test]
        public async Task ThrowsWhenNoMatchingAppointment()
        {
            Func<Task> testFunc = async () => await _classUnderTest.CreateSlotBooking("1/2020-01-01", 100001);

            await testFunc.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task GetSlotAppointment()
        {
            var date = DateTime.UtcNow.Date;
            var start = new DateTime().AddHours(8);
            var end = new DateTime().AddHours(12);
            var description = "AM";
            var workOrderRef = 100001;

            var ids = SeedAppointmentData("contractor", new DaySeedModel[]
            {
                new DaySeedModel(date.DayOfWeek, 1),
            }, new AppointmentSeedModel[]
            {
                new AppointmentSeedModel(description, start, end)
            });
            await _classUnderTest.CreateSlotBooking(GenerateAppointmentRef(ids.First(), date), workOrderRef);

            var appointment = await _classUnderTest.GetAppointment(workOrderRef);

            appointment.Description.Should().Be(description);
            appointment.Start.Should().Be(start);
            appointment.End.Should().Be(end);
            appointment.Date.Should().Be(date);
        }

        [Test]
        public async Task GetTimedAppointment()
        {
            var date = DateTime.UtcNow.Date;
            var start = DateTime.UtcNow.AddHours(8);
            var end = DateTime.UtcNow.AddHours(12);
            var description = Resources.ExternallyManagedAppointment;
            var workOrderRef = 100001;
            await CreateWorkOrder(workOrderRef);
            await _classUnderTest.SetTimedBooking(workOrderRef, start, end);

            var appointment = await _classUnderTest.GetAppointment(workOrderRef);

            appointment.Description.Should().Be(description);
            appointment.Start.Should().Be(start);
            appointment.End.Should().Be(end);
        }

        private static List<int> SeedAppointmentData(string contractor, DaySeedModel[] days, AppointmentSeedModel[] appointments)
        {
            List<int> bookableAppointmentsIds = new List<int>();

            foreach (var app in appointments)
            {
                var entry = InMemoryDb.Instance.AvailableAppointments.Add(new RepairsApi.V2.Infrastructure.Hackney.AvailableAppointment
                {
                    ContractorReference = contractor,
                    Description = app.Description,
                    StartTime = app.StartTime,
                    EndTime = app.EndTime,
                });

                foreach (var day in days)
                {
                    var innerEntry = InMemoryDb.Instance.AvailableAppointmentDays.Add(new RepairsApi.V2.Infrastructure.Hackney.AvailableAppointmentDay
                    {
                        AvailableAppointmentId = entry.Entity.Id,
                        AvailableCount = day.Count,
                        Day = day.Day
                    });

                    bookableAppointmentsIds.Add(innerEntry.Entity.Id);
                }

                InMemoryDb.Instance.SaveChanges();
            }

            return bookableAppointmentsIds;
        }

        [TearDown]
        public void TearDown()
        {
            InMemoryDb.Teardown();
        }
    }

}
