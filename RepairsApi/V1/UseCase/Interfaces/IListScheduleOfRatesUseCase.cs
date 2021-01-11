using RepairsApi.V1.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase.Interfaces
{
#nullable enable
    public interface IListScheduleOfRatesUseCase
    {
        Task<IList<ScheduleOfRatesModel>> Execute();
    }
}
