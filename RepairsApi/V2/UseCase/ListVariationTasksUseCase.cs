using RepairsApi.V2.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using RepairsApi.V2.Gateways;
using System.Linq;
using RepairsApi.V2.Factories;

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
            return tasks.Select(t => t.ToVariationsResponse()).ToList();
        }
    }
}
