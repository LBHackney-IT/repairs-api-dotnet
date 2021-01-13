using System.Collections.Generic;
using RepairsApi.V2.Boundary.Response;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IListWorkOrdersUseCase
    {
        IList<WorkOrderListItem> Execute();
    }

}
