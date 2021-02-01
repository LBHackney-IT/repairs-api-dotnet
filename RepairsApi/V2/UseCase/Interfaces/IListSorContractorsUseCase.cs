using RepairsApi.V2.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
    public interface IListSorContractorsUseCase
    {
        Task<IEnumerable<Contractor>> Execute(string propRef);
    }
}
