using RepairsApi.V2.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
#nullable enable
    public interface IListScheduleOfRatesUseCase
    {
        Task<IList<ScheduleOfRatesModel>> Execute();
    }
}