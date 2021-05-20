using System.Threading.Tasks;

namespace RepairsApi.V2.Email
{
    public interface IEmailService
    {
        Task SendMailAsync<TRequest>(TRequest request)
            where TRequest : EmailRequest;
    }
}
