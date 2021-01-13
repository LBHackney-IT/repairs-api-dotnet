using RepairsApi.V2.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public interface ITenancyGateway
    {
        Task<TenureInformation?> GetTenancyInformationAsync(string propertyReference);
    }
}
