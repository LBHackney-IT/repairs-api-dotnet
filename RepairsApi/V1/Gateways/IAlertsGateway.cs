using RepairsApi.V1.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public interface IAlertsGateway
    {
        Task<PropertyAlertList> GetAlertsAsync(string propertyReference);
    }
}
