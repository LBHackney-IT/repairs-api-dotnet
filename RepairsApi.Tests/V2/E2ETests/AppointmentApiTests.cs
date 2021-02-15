using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2;
using System;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.E2ETests
{
    public class AppointmentApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task BadRequestForNoWorkOrder()
        {
            var client = CreateClient();

            var result = await client.GetAsync(new Uri("/api/v2/appointments", UriKind.Relative));

            result.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task NotFoundForMissingWorkOrder()
        {
            var client = CreateClient();

            var result = await client.GetAsync(new Uri("/api/v2/appointments?workOrderReference=99999999", UriKind.Relative));

            result.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task ListForWorkOrder()
        {
            var client = CreateClient();

            var woRef = AddWorkOrder();
            var result = await client.GetAsync(new Uri($"/api/v2/appointments?workOrderReference={woRef}", UriKind.Relative));

            result.StatusCode.Should().Be(200);
        }

        private int AddWorkOrder()
        {
            int woRef = 0;
            WithContext(ctx =>
            {
                var entry = ctx.WorkOrders.Add(new RepairsApi.V2.Infrastructure.WorkOrder());

                ctx.SaveChanges();

                woRef = entry.Entity.Id;
            });
            return woRef;
        }

        [Test]
        public async Task ListForWorkOrderForDates()
        {
            var client = CreateClient();

            var woRef = AddWorkOrder();
            var toDate = DateTime.UtcNow.AddDays(1).ToString(DateConstants.DATEFORMAT);
            var fromDate = DateTime.UtcNow.AddDays(-1).ToString(DateConstants.DATEFORMAT);
            var result = await client.GetAsync(new Uri($"/api/v2/appointments?workOrderReference={woRef}&toDate={toDate}&fromDate={fromDate}", UriKind.Relative));

            result.StatusCode.Should().Be(200);
        }


        [Test]
        public async Task BadRequestForBadDateOrder()
        {
            var client = CreateClient();

            var woRef = AddWorkOrder();
            var toDate = DateTime.UtcNow.AddDays(-1).ToString(DateConstants.DATEFORMAT);
            var fromDate = DateTime.UtcNow.AddDays(1).ToString(DateConstants.DATEFORMAT);
            var result = await client.GetAsync(new Uri($"/api/v2/appointments?workOrderReference={woRef}&toDate={toDate}&fromDate={fromDate}", UriKind.Relative));

            result.StatusCode.Should().Be(400);
        }
    }
}
