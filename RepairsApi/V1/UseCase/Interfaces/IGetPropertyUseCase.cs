using RepairsApi.V1.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V1.UseCase.Interfaces
{
#nullable enable
    public interface IGetPropertyUseCase
    {
        Task<PropertyWithAlerts> ExecuteAsync(string propertyReference);
    }
}
