using RepairsApi.V1.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase.Interfaces
{
    public interface IListAlertsUseCase
    {
        Task<PropertyAlertList> ExecuteAsync(string propertyReference);
    }
}
