using RepairsApi.V1.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V1.Gateways
{
    public interface IPropertyGateway
    {
        Task<PropertyWithAlerts> GetByReferenceAsync(string propertyReference, bool includeAlerts);
        Task<PropertyAlertList> GetAlertsAsync(string propertyReference);
    }
}
