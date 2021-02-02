using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Controllers.Parameters;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Infrastructure;
using RepairsApi.V2.UseCase.Interfaces;

namespace RepairsApi.V2.UseCase
{
    public class ListWorkOrdersUseCase : IListWorkOrdersUseCase
    {
        private readonly IRepairsGateway _repairsGateway;
        private readonly IScheduleOfRatesGateway _scheduleOfRatesGateway;

        public ListWorkOrdersUseCase(IRepairsGateway repairsGateway, IScheduleOfRatesGateway scheduleOfRatesGateway)
        {
            _repairsGateway = repairsGateway;
            _scheduleOfRatesGateway = scheduleOfRatesGateway;
        }

        public async Task<IEnumerable<WorkOrderListItem>> Execute(WorkOrderSearchParameters searchParameters)
        {
            IEnumerable<WorkOrder> workOrders = await _repairsGateway.GetWorkOrders(await GetConstraints(searchParameters));

            return workOrders.Select(wo => wo.ToListItem())
                .OrderByDescending(wo => wo.DateRaised)
                .Skip((searchParameters.PageNumber - 1) * searchParameters.PageSize)
                .Take(searchParameters.PageSize)
                .ToList();
        }

        private async Task<Expression<Func<WorkOrder, bool>>[]> GetConstraints(WorkOrderSearchParameters searchParameters)
        {
            var result = new List<Expression<Func<WorkOrder, bool>>>();

            if (!string.IsNullOrWhiteSpace(searchParameters.PropertyReference))
            {
                result.Add(wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == searchParameters.PropertyReference));
            }

            if (!string.IsNullOrWhiteSpace(searchParameters.ContractorReference))
            {
                var relatedSorCodes = await _scheduleOfRatesGateway.GetContracts(searchParameters.ContractorReference);
                result.Add(wo =>
                    wo.WorkElements.Any(we =>
                        we.RateScheduleItem.Any(rsi =>
                            relatedSorCodes.Contains(rsi.ContractReference)
                        )
                    )
                );
            }

            return result.ToArray();
        }
    }
}
