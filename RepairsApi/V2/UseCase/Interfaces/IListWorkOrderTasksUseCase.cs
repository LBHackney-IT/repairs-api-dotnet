using RepairsApi.V2.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IListWorkOrderTasksUseCase
    {
        Task<IEnumerable<WorkOrderTask>> Execute(int id);
    }
}
