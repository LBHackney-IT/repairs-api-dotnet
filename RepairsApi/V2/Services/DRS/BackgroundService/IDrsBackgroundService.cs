using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;

namespace RepairsApi.V2.Generated.DRS.BackgroundService
{
    [ServiceContract(Namespace = "http://www.sx3.com/XMB-CONFIRM-BOOKINGS"), XmlSerializerFormat]
    public interface IDrsBackgroundService
    {
        [OperationContract]
        string ConfirmBookings(Bookings Bookings);
    }

    public class DrsBackgroundService : IDrsBackgroundService
    {
        public string ConfirmBookings(Bookings bookings)
        {
            Console.WriteLine(bookings.ToString());
            return bookings.ToString();
        }
    }
}
