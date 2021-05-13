using FluentAssertions;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Authorisation;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RepairsApi.V2.Helpers;
using WorkStatusCode = RepairsApi.V2.Infrastructure.WorkStatusCode;

namespace RepairsApi.Tests.E2ETests.Repairs
{
    public partial class RepairApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task UpdateCausesPendingApprovalOnWorkOrderWhenCostLimitExceeded()
        {
            // Arrange
            int workOrderId = await CreateVariation();

            var workOrder = GetWorkOrderFromDB(workOrderId, workOrder => workOrder.JobStatusUpdates.Load());

            // Assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationPendingApproval);
            workOrder.JobStatusUpdates[0].TypeCode.Should().Be(JobStatusUpdateTypeCode._180);
        }

        [TestCase(JobStatusUpdateTypeCode._10020, WorkStatusCode.VariationApproved)]
        [TestCase(JobStatusUpdateTypeCode._125, WorkStatusCode.VariationRejected)]
        public async Task WorkOrderRejectVariation(JobStatusUpdateTypeCode updateCode, WorkStatusCode expectedStatus)
        {
            // Arrange
            int workOrderId = await CreateVariation();

            SetUserRole(UserGroups.ContractManager);
            await UpdateJob(workOrderId, updateCode);

            var workOrder = GetWorkOrderFromDB(workOrderId);

            // Assert
            workOrder.StatusCode.Should().Be(expectedStatus);
        }

        [Test]
        public async Task ContractorAcknowledgeWorkOrderSetToInProgress()
        {
            // Arrange
            int workOrderId = await CreateVariation();

            SetUserRole(UserGroups.ContractManager);
            await UpdateJob(workOrderId, JobStatusUpdateTypeCode._10020);

            SetUserRole(UserGroups.Contractor);
            await UpdateJob(workOrderId, JobStatusUpdateTypeCode._10010);

            var workOrder = GetWorkOrderFromDB(workOrderId);

            // Assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.Open);
        }

        [TestCase(UserGroups.Contractor, JobStatusUpdateTypeCode._10020)]
        [TestCase(UserGroups.Agent, JobStatusUpdateTypeCode._10020)]
        [TestCase(UserGroups.Contractor, JobStatusUpdateTypeCode._125)]
        [TestCase(UserGroups.Agent, JobStatusUpdateTypeCode._125)]
        public async Task InvalidUserUpdatingVariationReturns401(string userGroup, JobStatusUpdateTypeCode updateCode)
        {
            // Arrange
            int workOrderId = await CreateVariation();

            SetUserRole(userGroup);
            var code = await UpdateJob(workOrderId, updateCode);

            var workOrder = GetWorkOrderFromDB(workOrderId);

            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationPendingApproval);
            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [TestCase(UserGroups.Agent)]
        [TestCase(UserGroups.ContractManager)]
        public async Task IvalidUserAcknowledgingWorkOrderReturns401(string userGroup)
        {
            // Arrange
            int workOrderId = await CreateVariation();

            SetUserRole(UserGroups.ContractManager);
            await UpdateJob(workOrderId, JobStatusUpdateTypeCode._10020);

            SetUserRole(userGroup);
            var code = await UpdateJob(workOrderId, JobStatusUpdateTypeCode._10010);

            var workOrder = GetWorkOrderFromDB(workOrderId);

            // Assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationApproved);
            code.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task GetWorkOrderVariationTasks()
        {
            // Arrange
            const int newItemQuantity = 100000;
            const string Notes = "notes";
            int workOrderId = await CreateVariation(newItemQuantity, Notes);

            SetUserRole(UserGroups.ContractManager);
            var (code, response) = await Get<GetVariationResponse>($"/api/v2/workOrders/{workOrderId}/variation-tasks");

            // Assert
            response.Tasks.ElementAt(1).VariedQuantity.Should().Be(newItemQuantity);
            response.Tasks.ElementAt(1).UnitCost.Should().NotBeNull();
            response.Notes.Should().EndWith(Notes);
            code.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task AddMultipleCodesInVariation()
        {
            string expectedCode = Guid.NewGuid().ToString();
            string otherExpectedCode = Guid.NewGuid().ToString();
            AddTestCode(expectedCode);
            AddTestCode(otherExpectedCode);
            var result = await CreateWorkOrder();

            await UpdateSorCodes(result.Id, jsu =>
            {
                jsu.MoreSpecificSORCode.RateScheduleItem.Add(new RateScheduleItem
                {
                    CustomCode = expectedCode,
                    CustomName = "customName",
                    Quantity = new Quantity { Amount = new List<double> { 1 } },
                });

                jsu.MoreSpecificSORCode.RateScheduleItem.Add(new RateScheduleItem
                {
                    CustomCode = otherExpectedCode,
                    CustomName = "customName",
                    Quantity = new Quantity { Amount = new List<double> { 1 } },
                });
            });

            var tasks = await GetTasks(result.Id);

            tasks.Should().ContainSingle(r => r.Code == expectedCode);
            tasks.Should().ContainSingle(r => r.Code == otherExpectedCode);
        }

        private async Task<int> CreateVariation(int newItemQuantity = 100000000, string notes = "notes")
        {
            string expectedCode = Guid.NewGuid().ToString();
            AddTestCode(expectedCode);
            var result = await CreateWorkOrder();

            await UpdateSorCodes(result.Id, jsu =>
            {
                jsu.MoreSpecificSORCode.RateScheduleItem.Add(new RateScheduleItem
                {
                    CustomCode = expectedCode,
                    CustomName = "customName",
                    Quantity = new Quantity { Amount = new List<double> { newItemQuantity } },
                });
                jsu.Comments = notes;
            });

            return result.Id;
        }
    }
}
