using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Gateways
{
    public interface IJobStatusUpdateGateway
    {
        Task<int> CreateJobStatusUpdate(JobStatusUpdate update);
    }
}
