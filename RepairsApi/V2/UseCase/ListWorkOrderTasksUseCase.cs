using RepairsApi.V2.Domain;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.UseCase.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase
{
    public class ListWorkOrderTasksUseCase : IListWorkOrderTasksUseCase
    {
        private readonly IRepairsGateway _repairsGatway;

        public ListWorkOrderTasksUseCase(IRepairsGateway repairsGatway)
        {
            _repairsGatway = repairsGatway;
        }

        public async Task<IEnumerable<WorkOrderTask>> Execute(int id)
        {
            var elements = await _repairsGatway.GetWorkElementsForWorkOrder(id);

            if (elements is null) throw new ResourceNotFoundException("work order not found");

            return elements.SelectMany(we => we.RateScheduleItem, (we, rsi) =>
            {
                double quantity = rsi.Quantity?.Amount ?? 0;

                return new WorkOrderTask
                {
                    Description = rsi.CustomName,
                    Quantity = quantity,
                    Cost = rsi.CodeCost,
                    Id = rsi.CustomCode,
                    Status = "Unknown",
                    DateAdded = rsi.DateCreated.GetValueOrDefault()
                };
            });
        }
    }
}
