using RepairsApi.V2.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IListSorTradesUseCase
    {
        Task<IEnumerable<SorTradeResponse>> Execute();
    }
}
