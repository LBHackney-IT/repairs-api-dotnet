using System.Threading.Tasks;

namespace RepairsApi.V2.Services
{
    public interface ICurrentUserLoader
    {
        Task LoadUser(string jwt);
    }
}
