using RepairsApi.V2.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public interface IAlertsGateway
    {
        Task<PropertyAlertList> GetLocationAlertsAsync(string propertyReference);
        Task<PersonAlertList> GetPersonAlertsAsync(string? tenancyReference);
    }
}
