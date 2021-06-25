using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using Party = RepairsApi.V2.Infrastructure.Party;

namespace RepairsApi.Tests.E2ETests
{
    public class AppointmentApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task BadRequestForNoWorkOrder()
        {
            var result = await Get("/api/v2/appointments");

            result.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UnAuthorisedOnList()
        {
            await AuthorisationHelper.VerifyContractorUnauthorised(
                CreateClient(),
                GetGroup(TestDataSeeder.Contractor),
                async client => await client.GetAsync(new Uri("/api/v2/appointments", UriKind.Relative)));
        }

        [Test]
        public async Task NotFoundForMissingWorkOrder()
        {
            var result = await Get("/api/v2/appointments?workOrderReference=99999999");

            result.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task ListForWorkOrder()
        {
            var woRef = AddWorkOrder();
            var result = await Get($"/api/v2/appointments?workOrderReference={woRef}");

            result.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ListForWorkOrderForDates()
        {
            var woRef = AddWorkOrder();
            var expectedAppointment = new AppointmentSeedModel("AM", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
            var (code, appointments) = await GetAppointments(woRef, expectedAppointment);

            code.Should().Be(HttpStatusCode.OK);
            var appointment = appointments.Should().ContainSingle().Which.Slots.Should().ContainSingle().Which;
            appointment.Description.Should().Be(expectedAppointment.Description);
            appointment.Start.Should().Be(expectedAppointment.StartTime.ToTime());
            appointment.End.Should().Be(expectedAppointment.EndTime.ToTime());
        }

        [Test]
        public async Task BookSlotAppointment()
        {
            var woRef = AddWorkOrder();
            var expectedAppointment = new AppointmentSeedModel("AM", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
            var (code, appointments) = await GetAppointments(woRef, expectedAppointment);

            code.Should().Be(HttpStatusCode.OK);
            var appointment = appointments.Single().Slots.Single();

            var request = new RequestAppointment
            {
                WorkOrderReference = new Reference
                {
                    ID = woRef.ToString()
                },
                AppointmentReference = new Reference
                {
                    ID = appointment.Reference
                }
            };
            code = await Post($"/api/v2/appointments", request);

            code.Should().Be(HttpStatusCode.OK);

            using var ctx = GetContext();
            var db = ctx.DB;
            var bookedAppointment = await db.Appointments.SingleAsync(a => a.WorkOrderId == woRef);
            bookedAppointment.StartTime.Should().Be(expectedAppointment.StartTime);
            bookedAppointment.EndTime.Should().Be(expectedAppointment.EndTime);
        }

        [Test]
        public async Task ReBookSlotAppointment()
        {
            var woRef = AddWorkOrder();
            var initialSlotSeed = new AppointmentSeedModel("AM", DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
            var expectedSlotSeed = new AppointmentSeedModel("PM", DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6));
            var allAppointments = new[] { initialSlotSeed, expectedSlotSeed };
            var (code, appointments) = await GetAppointments(woRef, allAppointments);
            code.Should().Be(HttpStatusCode.OK);

            var appointmentSlots = appointments.Single().Slots;
            appointmentSlots.Should().HaveCount(2);
            var initialSlot = appointmentSlots.Single(s => s.Description == initialSlotSeed.Description);
            var expectedSlot = appointmentSlots.Single(s => s.Description == expectedSlotSeed.Description);

            await RequestAppointment(woRef, initialSlot.Reference);
            await RequestAppointment(woRef, expectedSlot.Reference);

            using var ctx = GetContext();
            var db = ctx.DB;
            var bookedAppointment = await db.Appointments.SingleAsync(a => a.WorkOrderId == woRef);
            bookedAppointment.StartTime.Should().Be(expectedSlotSeed.StartTime);
            bookedAppointment.EndTime.Should().Be(expectedSlotSeed.EndTime);
        }

        private async Task RequestAppointment(int woRef, string appointmentReference)
        {
            var request = new RequestAppointment
            {
                WorkOrderReference = new Reference
                {
                    ID = woRef.ToString()
                },
                AppointmentReference = new Reference
                {
                    ID = appointmentReference
                }
            };
            var code = await Post($"/api/v2/appointments", request);
            code.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task BadRequestWhenBookSlotAppointmentWithBadWorkOrderId()
        {
            var request = new RequestAppointment
            {
                WorkOrderReference = new Reference
                {
                    ID = "notANumber"
                },
                AppointmentReference = new Reference
                {
                    ID = "string"
                }
            };
            var code = await Post($"/api/v2/appointments", request);

            code.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task NotFoundWhenBookSlotAppointmentWithUnknownWorkOrderId()
        {
            var request = new RequestAppointment
            {
                WorkOrderReference = new Reference
                {
                    ID = "1337"
                },
                AppointmentReference = new Reference
                {
                    ID = "string"
                }
            };
            var code = await Post($"/api/v2/appointments", request);

            code.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<(HttpStatusCode statusCode, List<AppointmentDayViewModel> response)> GetAppointments(int woRef, params AppointmentSeedModel[] expectedAppointments)
        {
            SeedAppointmentData("contractor", new[]
            {
                new DaySeedModel(DateTime.UtcNow.DayOfWeek, 1),
            }, expectedAppointments);
            var toDate = DateTime.UtcNow.AddDays(1).ToDate();
            var fromDate = DateTime.UtcNow.AddDays(-1).ToDate();

            return await Get<List<AppointmentDayViewModel>>($"/api/v2/appointments?workOrderReference={woRef}&toDate={toDate}&fromDate={fromDate}");
        }


        [Test]
        public async Task BadRequestForBadDateOrder()
        {
            var woRef = AddWorkOrder();
            var toDate = DateTime.UtcNow.AddDays(-1).ToDate();
            var fromDate = DateTime.UtcNow.AddDays(1).ToDate();
            var result = await Get($"/api/v2/appointments?workOrderReference={woRef}&toDate={toDate}&fromDate={fromDate}");

            result.Should().Be(HttpStatusCode.BadRequest);
        }

        private int AddWorkOrder()
        {
            using var ctx = GetContext();
            var db = ctx.DB;
            var entry = db.WorkOrders.Add(new RepairsApi.V2.Infrastructure.WorkOrder
            {
                AssignedToPrimary = new Party
                {
                    ContractorReference = "contractor"
                }
            });

            db.SaveChanges();

            return entry.Entity.Id;
        }

        private List<int> SeedAppointmentData(string contractor, DaySeedModel[] days, AppointmentSeedModel[] appointments)
        {
            using var ctx = GetContext();
            var bookableAppointmentsIds = new List<int>();

            foreach (var app in appointments)
            {
                var entry = ctx.DB.AvailableAppointments.Add(new RepairsApi.V2.Infrastructure.Hackney.AvailableAppointment
                {
                    ContractorReference = contractor,
                    Description = app.Description,
                    StartTime = app.StartTime,
                    EndTime = app.EndTime,
                });

                foreach (var day in days)
                {
                    var innerEntry = ctx.DB.AvailableAppointmentDays.Add(new RepairsApi.V2.Infrastructure.Hackney.AvailableAppointmentDay
                    {
                        AvailableAppointmentId = entry.Entity.Id,
                        AvailableCount = day.Count,
                        Day = day.Day
                    });

                    bookableAppointmentsIds.Add(innerEntry.Entity.Id);
                }

                ctx.DB.SaveChanges();
            }

            return bookableAppointmentsIds;
        }
    }
}
