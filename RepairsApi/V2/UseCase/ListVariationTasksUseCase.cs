using RepairsApi.V2.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using RepairsApi.V2.Gateways;
using System.Linq;

namespace RepairsApi.V2.UseCase
{
    public class ListVariationTasksUseCase : IListVariationTasksUseCase
    {
        private readonly IJobStatusUpdateGateway _jobStatusUpdateGateway;
        public ListVariationTasksUseCase(IJobStatusUpdateGateway jobStatusUpdateGateway)
        {
            _jobStatusUpdateGateway = jobStatusUpdateGateway;
        }
        public async Task<IEnumerable<VariationTasksModel>> Execute(int workOrderId)
        {
            var tasks = await _jobStatusUpdateGateway.SelectWorkOrderVariationTasks(workOrderId);

            return tasks.Select(t => new VariationTasksModel
            {

                Id = t.Id.ToString(),
                Code = t.CustomCode,
                Description = t.CustomName,
                UnitCost = t.CodeCost,
                OldQuantity = t.OriginalQuantity,
                NewQuantity = t.Quantity?.Amount
            }
            ).ToList();

        }
    }
}
