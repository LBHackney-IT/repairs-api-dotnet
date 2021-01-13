using RepairsApi.V2.Domain;
using System.Threading.Tasks;

namespace RepairsApi.V2.UseCase.Interfaces
{
#nullable enable
    public interface IGetPropertyUseCase
    {
        Task<PropertyWithAlerts> ExecuteAsync(string propertyReference);
    }
}
