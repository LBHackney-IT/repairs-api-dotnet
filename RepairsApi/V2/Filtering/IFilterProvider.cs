using RepairsApi.V2.Configuration;
using System.Threading.Tasks;

namespace RepairsApi.V2.Filtering
{
    public interface IFilterProvider
    {
        Task<ModelFilterConfiguration> GetFilter();
    }
}
