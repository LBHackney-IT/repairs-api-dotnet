using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace RepairsApi.V2.Notifications
{
    public interface INotificationHandler<T>
        where T : INotification
    {
        Task Notify(T data); 
    }

    [SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Enforces bespoke types for notification events")]
    public interface INotification
    {

    }
}
