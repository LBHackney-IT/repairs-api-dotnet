using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Factories;
using RepairsApi.V2.Gateways;
using RepairsApi.V2.Helpers;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<GetVariationResponse> Execute(int workOrderId)
        {
            var workOrder = await _repairsGateway.GetWorkOrder(workOrderId);
            workOrder.VerifyCanGetVariation();

            var workOrderTasks = workOrder.WorkElements.SelectMany(we => we.RateScheduleItem);
            var variation = await _jobStatusUpdateGateway.GetOutstandingVariation(workOrderId);
            var variationTasks = variation.MoreSpecificSORCode.RateScheduleItem;

            return new GetVariationResponse
            {
                Notes = variation.Comments,
                Tasks = MapTasks(variationTasks, workOrderTasks),
                AuthorName = variation.AuthorName,
                VariationDate = variation.EventTime ?? DateTime.UtcNow
            };
        }

        private static IEnumerable<VariationTasksModel> MapTasks(List<RateScheduleItem> variationTasks, IEnumerable<Infrastructure.RateScheduleItem> workOrderTasks)
        {
            return from vTask in variationTasks
                   join wTask in workOrderTasks on vTask.OriginalId equals wTask.Id into gj
                   from groupTask in gj.DefaultIfEmpty()
                   select new VariationTasksModel
                   {
                       Id = vTask.OriginalId.ToString(),
                       Code = vTask.CustomCode,
                       Description = vTask.CustomName,
                       UnitCost = vTask.CodeCost,
                       OriginalQuantity = groupTask?.OriginalQuantity ?? 0,
                       CurrentQuantity = groupTask?.Quantity?.Amount ?? 0,
                       VariedQuantity = vTask.Quantity?.Amount
                   };
        }
    }
}
