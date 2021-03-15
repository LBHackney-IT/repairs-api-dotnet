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

        public ListWorkOrdersUseCase(IRepairsGateway repairsGateway)
        {
            _repairsGateway = repairsGateway;
        }

        public async Task<IEnumerable<WorkOrderListItem>> Execute(WorkOrderSearchParameters searchParameters)
        {
            IEnumerable<WorkOrder> workOrders = await _repairsGateway.GetWorkOrders(GetConstraints(searchParameters));

            return workOrders.Select(wo => wo.ToListItem())
                .OrderBy(wo => wo.Status)
                .ThenByDescending(wo => wo.DateRaised)
                .Skip((searchParameters.PageNumber - 1) * searchParameters.PageSize)
                .Take(searchParameters.PageSize)
                .ToList();
        }

        private static Expression<Func<WorkOrder, bool>>[] GetConstraints(WorkOrderSearchParameters searchParameters)
        {
            var result = new List<Expression<Func<WorkOrder, bool>>>();

            if (!string.IsNullOrWhiteSpace(searchParameters.PropertyReference))
            {
                result.Add(wo => wo.Site.PropertyClass.Any(pc => pc.PropertyReference == searchParameters.PropertyReference));
            }

            if (!string.IsNullOrWhiteSpace(searchParameters.ContractorReference))
            {
                result.Add(wo => wo.AssignedToPrimary.ContractorReference == searchParameters.ContractorReference);
            }

            if(searchParameters.StatusCode > 0 && Enum.IsDefined(typeof(WorkStatusCode), searchParameters.StatusCode))
            {
                result.Add(wo => wo.StatusCode == (WorkStatusCode) Enum.Parse(typeof(WorkStatusCode), searchParameters.StatusCode.ToString()));
            }

            return result.ToArray();
        }
    }
}
