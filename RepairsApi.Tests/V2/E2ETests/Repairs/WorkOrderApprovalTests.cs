using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.E2ETests.Repairs
{
    public partial class RepairApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task ScheduledRepairStatusIsPendingIfAboveSpendLimit()
        {
            var id = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });
            var wo = GetWorkOrderFromDB(id);

            wo.StatusCode.Should().Be(WorkStatusCode.PendingApproval);
        }

        [TestCase(UserGroups.Agent)]
        [TestCase(UserGroups.Contractor)]
        public async Task ApprovePendingWorkOrderShould401ForUnAuthorised(string userGroup)
        {
            SetUserRole(userGroup);
            var id = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            var code = await UpdateJob(id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._200);

            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [TestCase(UserGroups.Agent)]
        [TestCase(UserGroups.Contractor)]
        public async Task RejectPendingWorkOrderShould401ForUnAuthorised(string userGroup)
        {
            SetUserRole(userGroup);
            var id = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            var code = await UpdateJob(id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._190);

            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApprovePendingWorkOrder()
        {
            SetUserRole(UserGroups.Agent);
            var id = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            SetUserRole(UserGroups.ContractManager);
            var code = await UpdateJob(id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._200);

            var wo = GetWorkOrderFromDB(id);
            code.Should().Be(HttpStatusCode.OK);
            wo.StatusCode.Should().Be(WorkStatusCode.Open);
        }

        [Test]
        public async Task RejectPendingWorkOrder()
        {
            SetUserRole(UserGroups.Agent);
            var id = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            SetUserRole(UserGroups.ContractManager);
            var code = await UpdateJob(id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._190);

            var wo = GetWorkOrderFromDB(id);
            code.Should().Be(HttpStatusCode.OK);
            wo.StatusCode.Should().Be(WorkStatusCode.Canceled);
        }
    }
}
