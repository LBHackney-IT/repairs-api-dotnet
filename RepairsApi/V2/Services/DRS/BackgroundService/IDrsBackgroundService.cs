using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;

namespace RepairsApi.V2.Generated.DRS.BackgroundService
{
    [ServiceContract(Namespace = "http://www.sx3.com/XMB-CONFIRM-BOOKINGS"), XmlSerializerFormat]
    public interface IDrsBackgroundService
    {
        [OperationContract]
        // ReSharper disable once InconsistentNaming
        string ConfirmBookings(Bookings Bookings);
    }

}
