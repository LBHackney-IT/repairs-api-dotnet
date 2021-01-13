using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V1.Boundary.Response;

namespace RepairsApi.V1.UseCase.Interfaces
{
    public interface IListWorkOrdersUseCase
    {
        IList<WorkOrderListItem> Execute();
    }

}
