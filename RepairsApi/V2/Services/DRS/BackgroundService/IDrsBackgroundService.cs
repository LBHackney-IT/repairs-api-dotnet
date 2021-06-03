using System.ServiceModel;
using System.Xml.Serialization;

namespace RepairsApi.V2.Generated.DRS.BackgroundService
{
    [ServiceContract(Namespace = "")]
    public interface IDrsBackgroundService
    {
        [OperationContract(Name = "ns1bookingConfirmation")]
        string ConfirmBooking([XmlElement(ElementName = "bookingConfirmation")] bookingConfirmation bookingConfirmation);
    }
}
