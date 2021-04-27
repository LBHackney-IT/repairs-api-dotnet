using RepairsApi.V2.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobStatusUpdateTypeCode = RepairsApi.V2.Generated.JobStatusUpdateTypeCode;

namespace RepairsApi.V2.Gateways
{
    public interface IJobStatusUpdateGateway
    {
        Task<int> CreateJobStatusUpdate(JobStatusUpdate update);
        Task<JobStatusUpdate> GetOutstandingVariation(int workOrderId);
        Task<IList<RateScheduleItem>> SelectWorkOrderVariationTasks(int workOrderId);
    }
}
