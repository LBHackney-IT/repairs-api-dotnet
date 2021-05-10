using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.Tests.E2ETests.Repairs
{
    public partial class RepairApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task ScheduledRepairStatusIsPendingIfAboveSpendLimit()
        {
            var result = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });
            var wo = GetWorkOrderFromDB(result.Id);

            wo.StatusCode.Should().Be(WorkStatusCode.PendingApproval);
        }

        [TestCase(UserGroups.Agent)]
        [TestCase(UserGroups.Contractor)]
        [TestCase(UserGroups.ContractManager)]
        public async Task ApprovePendingWorkOrderShould401ForUnAuthorised(string userGroup)
        {
            var result = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            SetUserRole(userGroup);
            var code = await UpdateJob(result.Id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._200);

            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [TestCase(UserGroups.Agent)]
        [TestCase(UserGroups.Contractor)]
        [TestCase(UserGroups.ContractManager)]
        public async Task RejectPendingWorkOrderShould401ForUnAuthorised(string userGroup)
        {
            var result = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            SetUserRole(userGroup);
            var code = await UpdateJob(result.Id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._190);

            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task ApprovePendingWorkOrder()
        {
            SetUserRole(UserGroups.Agent);
            var result = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            SetUserRole(UserGroups.AuthorisationManager);
            var code = await UpdateJob(result.Id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._200);

            var wo = GetWorkOrderFromDB(result.Id);
            code.Should().Be(HttpStatusCode.OK);
            wo.StatusCode.Should().Be(WorkStatusCode.Open);
        }

        [Test]
        public async Task RejectPendingWorkOrder()
        {
            SetUserRole(UserGroups.Agent);
            var result = await CreateWorkOrder(r => r.WorkElement.Single().RateScheduleItem.Single().Quantity.Amount = new List<double> { 50000 });

            SetUserRole(UserGroups.AuthorisationManager);
            var code = await UpdateJob(result.Id, RepairsApi.V2.Generated.JobStatusUpdateTypeCode._190);

            var wo = GetWorkOrderFromDB(result.Id);
            code.Should().Be(HttpStatusCode.OK);
            wo.StatusCode.Should().Be(WorkStatusCode.Canceled);
        }
    }
}
