using RepairsApi.V1.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public interface ITenancyGateway
    {
        Task<TenureInformation?> GetTenancyInformationAsync(string propertyReference);
    }
}
