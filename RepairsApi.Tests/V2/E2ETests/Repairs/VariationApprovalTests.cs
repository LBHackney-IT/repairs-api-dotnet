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

using WorkStatusCode = RepairsApi.V2.Infrastructure.WorkStatusCode;

namespace RepairsApi.Tests.V2.E2ETests.Repairs
{
    public partial class RepairApiTests : MockWebApplicationFactory
    {
        [Test]
        public async Task UpdateCausesPendingApprovalOnWorkOrderWhenCostLimitExceeded()
        {
            // Arrange
            int workOrderId = await CreatePendingVariation();

            var workOrder = GetWorkOrderFromDB(workOrderId, workOrder => workOrder.JobStatusUpdates.Load());

            // Assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationPendingApproval);
            workOrder.JobStatusUpdates[0].TypeCode.Should().Be(JobStatusUpdateTypeCode._180);
        }

        [Test]
        public async Task WorkOrderRejectVariation()
        {
            // Arrange
            int workOrderId = await CreatePendingVariation();

            SetUserRole(UserGroups.ContractManager);
            await UpdateJob(workOrderId, JobStatusUpdateTypeCode._125);

            var workOrder = GetWorkOrderFromDB(workOrderId);

            // Assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationRejected);
        }

        [Test]
        public async Task WorkOrderApproveVariation()
        {
            // Arrange
            int workOrderId = await CreatePendingVariation();

            SetUserRole(UserGroups.ContractManager);
            await UpdateJob(workOrderId, JobStatusUpdateTypeCode._10020);

            var workOrder = GetWorkOrderFromDB(workOrderId);

            // Assert
            workOrder.StatusCode.Should().Be(WorkStatusCode.VariationApproved);
        }

        [Test]
        public async Task ContractorAcknowledgeWorkOrderSetToInProgress()
        {
            // Arrange
            int workOrderId = await CreatePendingVariation();

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
            int workOrderId = await CreatePendingVariation();

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
            int workOrderId = await CreatePendingVariation();

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
            int workOrderId = await CreatePendingVariation(newItemQuantity);

            SetUserRole(UserGroups.ContractManager);
            var (code, response) = await Get<List<VariationTasksModel>>($"/api/v2/workOrders/{workOrderId}/variation-tasks");

            // Assert
            response[1].NewQuantity.Should().Be(newItemQuantity);
            code.Should().Be(HttpStatusCode.OK);
        }

        private async Task<int> CreatePendingVariation(int newItemQuantity = 100000000)
        {
            string expectedCode = Guid.NewGuid().ToString();
            AddTestCode(expectedCode);
            var workOrderId = await CreateWorkOrder();

            await UpdateJob(workOrderId, jsu =>
            {
                jsu.MoreSpecificSORCode.RateScheduleItem.Add(new RateScheduleItem
                {
                    CustomCode = expectedCode,
                    CustomName = "customName",
                    Quantity = new Quantity { Amount = new List<double> { newItemQuantity } },
                });
            });
            return workOrderId;
        }
    }
}
