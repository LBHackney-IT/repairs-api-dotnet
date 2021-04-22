using RepairsApi.V2.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using RepairsApi.V2.Gateways;
using System.Linq;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.UseCase
{
    public class ListVariationTasksUseCase : IListVariationTasksUseCase
    {
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;
        private readonly IRepairsGateway _repairsGateway;
        public ListVariationTasksUseCase(IJobStatusUpdateGateway jobStatusUpdateGateway,
            IRepairsGateway repairsGateway)
        {
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
            _repairsGateway = repairsGateway;
        }
        public async Task<IEnumerable<VariationTasksModel>> Execute(int workOrderId)
        {
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            if (workOrder.StatusCode != WorkStatusCode.PendApp)
                throw new InvalidOperationException("This action is not permitted");

            var workOrderTasks = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem);
            var variationTasks = await _jobStatusUpdateGateway.SelectWorkOrderVariationTasks(workOrderId);

            foreach (var task in variationTasks)
            {
                workOrderTasks.Where(t => t.Id == task.OriginalId)
                    .ToList()
                    .Select(t =>
                    {
                        task.OriginalQuantity = t.OriginalQuantity;
                        return t;
                    });
            }

            return variationTasks.Select(t => t.ToVariationsResponse()).ToList();
        }
    }
}
