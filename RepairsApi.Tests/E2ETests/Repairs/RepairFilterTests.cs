using FluentAssertions;
using NUnit.Framework;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Filtering;
using RepairsApi.V2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RepairsApi.Tests.E2ETests.Repairs
{
    public partial class RepairApiTests
    {
        [Test]
        public async Task GetFilteredListOfWorkOrders_Status()
        {
            // Arrange
            var openWorkOrderResult = await CreateWorkOrder();
            var completedWorkOrderResult = await CreateWorkOrder();
            await CancelWorkOrder(completedWorkOrderResult.Id);

            await TestWorkOrderFilter($"StatusCode={(int) WorkStatusCode.Open}", openWorkOrderResult.Id, $"StatusCode={(int) WorkStatusCode.Canceled}", completedWorkOrderResult.Id);
        }

        [Test]
        public async Task GetFilteredListOfWorkOrders_Priority()
        {
            // Arrange
            var urgentResult = await CreateWorkOrder(sr => sr.Priority.PriorityCode = 3);
            var immediateResult = await CreateWorkOrder(sr => sr.Priority.PriorityCode = 0);

            await TestWorkOrderFilter("Priorities=3", urgentResult.Id, "Priorities=0", immediateResult.Id);
        }

        [Test]
        public async Task GetFilteredListOfWorkOrders_Trades()
        {
            var filters = await GetFilters(FilterSectionConstants.Trades);
            string tradeCode1 = filters[0].Key;
            string tradeCode2 = filters[1].Key;

            // Arrange
            var trade1 = await CreateWorkOrder(sr => sr.WorkElement.First().Trade.First().CustomCode = tradeCode1);
            var trade2 = await CreateWorkOrder(sr => sr.WorkElement.First().Trade.First().CustomCode = tradeCode2);

            await TestWorkOrderFilter($"TradeCodes={tradeCode1}", trade1.Id, $"TradeCodes={tradeCode2}", trade2.Id);
        }

        [Test]
        public async Task GetFilteredListOfWorkOrders_Contractors()
        {
            SetupSoapMock();

            var filters = await GetFilters(FilterSectionConstants.Contractors);
            string contractorCode1 = filters[0].Key;
            string contractorCode2 = filters[1].Key;

            // Arrange
            var contractor1 = await CreateWorkOrder(sr => sr.AssignedToPrimary.Organization.Reference.First().ID = contractorCode1);
            var contractor2 = await CreateWorkOrder(sr => sr.AssignedToPrimary.Organization.Reference.First().ID = contractorCode2);

            await TestWorkOrderFilter($"ContractorReference={contractorCode1}", contractor1.Id, $"ContractorReference={contractorCode2}", contractor2.Id);
        }

        [Test]
        public async Task GetSortedListOfWorkOrder()
        {
            await Run(() => CreateWorkOrder(), 50);

            var (ascendingDode, ascendingResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?PageSize=50&sort=dateraised:asc");
            var (descendingCode, descendingResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?PageSize=50&sort=dateraised:desc");

            ascendingResponse.Should().BeInAscendingOrder(wo => wo.DateRaised);
            descendingResponse.Should().BeInDescendingOrder(wo => wo.DateRaised);
        }

        private static async Task Run(Func<Task> action, int times)
        {
            for (int i = 0; i < times; i++)
            {
                await action();
            }
        }

        private async Task TestWorkOrderFilter(string query1, int id1, string query2, int id2)
        {
            // Act
            var (firstCode, firstResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?PageSize=50&{query1}");
            var (secondCode, secondResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?PageSize=50&{query2}");
            var (multiCode, multiResponse) = await Get<List<WorkOrderListItem>>($"/api/v2/workOrders?PageSize=50&{query1}&{query2}");

            // Assert
            firstCode.Should().Be(HttpStatusCode.OK);
            firstResponse.Should().ContainSingle(wo => wo.Reference == id1);
            firstResponse.Should().NotContain(wo => wo.Reference == id2);

            secondCode.Should().Be(HttpStatusCode.OK);
            secondResponse.Should().ContainSingle(wo => wo.Reference == id2);
            secondResponse.Should().NotContain(wo => wo.Reference == id1);

            multiCode.Should().Be(HttpStatusCode.OK);
            multiResponse.Should().ContainSingle(wo => wo.Reference == id1);
            multiResponse.Should().ContainSingle(wo => wo.Reference == id2);
        }

        private async Task<List<FilterOption>> GetFilters(string section)
        {
            var (_, result) = await Get<Dictionary<string, List<FilterOption>>>("/api/v2/filter/WorkOrder");

            return result[section];
        }
    }
}
