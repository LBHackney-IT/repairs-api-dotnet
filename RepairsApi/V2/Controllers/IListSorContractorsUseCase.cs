using RepairsApi.V2.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IListSorContractorsUseCase
    {
        Task<IEnumerable<Contractor>> Execute();
    }
}
