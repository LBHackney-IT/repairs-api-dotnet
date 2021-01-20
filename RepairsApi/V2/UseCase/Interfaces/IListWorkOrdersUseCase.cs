using System.Collections.Generic;
using System.Threading.Tasks;
using RepairsApi.V2.Boundary.Response;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.Controllers.Parameters;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IListWorkOrdersUseCase
    {
        Task<IEnumerable<WorkOrderListItem>> Execute(WorkOrderSearchParameters parameters);
    }

}
