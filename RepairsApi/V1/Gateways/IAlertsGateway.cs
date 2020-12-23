using RepairsApi.V1.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public interface IAlertsGateway
    {
        Task<PropertyAlertList> GetLocationAlertsAsync(string propertyReference);
        Task<PersonAlertList> GetPersonAlertsAsync(string? tenancyReference);
    }
}
