using System.Threading.Tasks;

namespace RepairsApi.V2.Controllers
{
    public interface IListSorTradesUseCase
    {
        Task<object> Execute();
    }
}