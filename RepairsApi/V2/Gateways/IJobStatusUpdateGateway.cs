using System.Threading.Tasks;
using RepairsApi.V2.Generated;
using JobStatusUpdate = RepairsApi.V2.Infrastructure.JobStatusUpdate;

namespace RepairsApi.V2.Gateways
{
    public interface IJobStatusUpdateGateway
    {
        Task<int> CreateJobStatusUpdate(JobStatusUpdate update);
        Task<JobStatusUpdate> SelectLastJobStatusUpdate(JobStatusUpdateTypeCode typeCode, int workOrderId);
    }
}
