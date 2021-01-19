using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
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

        public async Task<IEnumerable<WorkOrderListItem>> Execute(WorkOrderSearchParamters searchParamters)
        {
            IEnumerable<WorkOrder> workOrders = await _repairsGateway.GetWorkOrders(GetConstraints(searchParamters));

            return workOrders.Select(wo => wo.ToListItem())
                .OrderByDescending(wo => wo.DateRaised)
                .ToList();
        }

        private Expression<Func<WorkOrder, bool>>[] GetConstraints(WorkOrderSearchParamters searchParamters)
        {
            List<Expression<Func<WorkOrder, bool>>> result = new List<Expression<Func<WorkOrder, bool>>>();

            if (!string.IsNullOrWhiteSpace(searchParamters.PropertyReference))
            {
                result.Add(wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == searchParamters.PropertyReference));
            }

            if (!string.IsNullOrWhiteSpace(searchParamters.ContractorReference))
            {
                result.Add(wo => wo.WorkElements.Any(we => we.RateScheduleItem.Any(rsi => _scheduleOfRatesGateway.GetContractorReference(rsi.CustomCode).Result == searchParamters.ContractorReference)));
            }

            return result.ToArray();
        }
    }
}
