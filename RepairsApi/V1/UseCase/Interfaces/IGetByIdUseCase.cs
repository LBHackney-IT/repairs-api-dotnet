using RepairsApi.V1.Boundary.Response;

namespace RepairsApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
