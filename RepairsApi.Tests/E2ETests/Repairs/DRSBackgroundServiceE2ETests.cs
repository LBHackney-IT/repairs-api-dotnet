using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RepairsApi.Tests.Helpers.StubGeneration;
using RepairsApi.V2;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Generated;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase;
using V2_Generated_DRS;
using Appointment = RepairsApi.V2.Infrastructure.Hackney.Appointment;
using RateScheduleItem = RepairsApi.V2.Generated.RateScheduleItem;

namespace RepairsApi.Tests.E2ETests.Repairs
{
    public class DRSBackgroundServiceE2ETests : MockWebApplicationFactory
    {
        [Test]
        public async Task CreatesAppointment()
        {
            var startTime = DateTime.UtcNow;
            startTime = startTime.AddTicks(-(startTime.Ticks % TimeSpan.TicksPerSecond));
            var endTime = startTime.AddHours(5);
            SetupSoapMock(startTime, endTime);

            SetUserRole(UserGroups.Agent);
            var result = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);

            SetUserRole(UserGroups.Service);
            var response = await CreateAppointment(result, startTime, endTime);

            var appointment = GetAppointmentFromDB(result.Id);

            response.Should().NotBeNull();
            response.InnerText.Should().Be(Resources.DrsBackgroundService_BookingAccepted);
            appointment.Should().NotBeNull();
            appointment.StartTime.Should().Be(startTime);
            appointment.EndTime.Should().Be(endTime);
        }

        [Test]
        public async Task UpdatesAppointment()
        {
            var startTime1 = DateTime.UtcNow;
            startTime1 = startTime1.AddTicks(-(startTime1.Ticks % TimeSpan.TicksPerSecond));
            var endTime1 = startTime1.AddHours(5);
            var startTime2 = startTime1.AddDays(7);
            var endTime2 = endTime1.AddDays(7);
            SetupSoapMock(startTime2, endTime2);

            SetUserRole(UserGroups.Agent);
            var result = await CreateWorkOrder(wo => wo.AssignedToPrimary.Organization.Reference.First().ID = TestDataSeeder.DRSContractor);

            // create first appointment
            SetUserRole(UserGroups.Service);
            await CreateAppointment(result, startTime1, endTime1);

            // update the appointment
            var response = await CreateAppointment(result, startTime2, endTime2);

            var appointment = GetAppointmentFromDB(result.Id);
            response.Should().NotBeNull();
            response.InnerText.Should().Be(Resources.DrsBackgroundService_BookingAccepted);
            appointment.Should().NotBeNull();
            appointment.StartTime.Should().Be(startTime2);
            appointment.EndTime.Should().Be(endTime2);
        }

        private async Task<XmlElement> CreateAppointment(CreateOrderResult result, DateTime startTime, DateTime endTime)
        {

            result.ExternallyManagedAppointment.Should().BeTrue();

            var drsConfirmBooking = Requests.DRSConfirmBooking;
            drsConfirmBooking = drsConfirmBooking.Replace("{{workOrderId}}", result.Id.ToString());
            drsConfirmBooking = drsConfirmBooking.Replace("{{planningWindowStart}}", startTime.ToString("s"));
            drsConfirmBooking = drsConfirmBooking.Replace("{{planningWindowEnd}}", endTime.ToString("s"));
            var (_, response) = await SoapPost("/Service.asmx", drsConfirmBooking);
            return response;
        }

        private async Task<(HttpStatusCode statusCode, XmlElement response)> SoapPost(string uri, string data)
        {
            var client = CreateClient();

            using var content = new StringContent(data, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(new Uri(uri, UriKind.Relative), content);

            var response = await ProcessResponse(result);
            return (result.StatusCode, response);
        }

        private static async Task<XmlElement> ProcessResponse(HttpResponseMessage result)
        {
            var responseContent = await result.Content.ReadAsStringAsync();

            var doc = new XmlDocument();
            doc.LoadXml(responseContent);

            return doc.DocumentElement;

        }

        private async Task<CreateOrderResult> CreateWorkOrder(Action<ScheduleRepair> interceptor = null)
        {
            var request = WorkOrderHelpers.CreateWorkOrderGenerator<ScheduleRepair>()
                .AddValue(new List<double>
                {
                    1
                }, (RateScheduleItem rsi) => rsi.Quantity.Amount)
                .Generate();

            interceptor?.Invoke(request);

            var (_, response) = await Post<CreateOrderResult>("/api/v2/workOrders/schedule", request);

            return response;
        }

        public Appointment GetAppointmentFromDB(int workOrderId, Action<Appointment> modifier = null)
        {
            using var ctx = GetContext();
            var db = ctx.DB;
            var repair = db.Appointments.SingleOrDefault(a => a.WorkOrderId == workOrderId);
            modifier?.Invoke(repair);
            return repair;
        }
    }
}
