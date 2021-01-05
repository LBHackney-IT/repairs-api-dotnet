using RepairsApi.V1.Domain.Repair;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public interface IRepairsGateway
    {
        Task CreateWorkOrder(WorkOrder raiseRepair);
    }
}
