using System.Threading.Tasks;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Services
{
    public interface IDrsService
    {
        Task OpenSession();
        Task CreateOrder(WorkOrder workOrder);
    }

}
