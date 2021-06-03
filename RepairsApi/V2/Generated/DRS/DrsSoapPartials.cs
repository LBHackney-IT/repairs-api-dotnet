using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Services;

namespace V2_Generated_DRS
{
    public partial class SOAPClient
    {
        private const int MaxReceivedMessageSize = 262144;
        public SOAPClient(IOptions<DrsOptions> drsOptions) :
            this(new BasicHttpsBinding
            {
                MaxReceivedMessageSize = MaxReceivedMessageSize
            }, new EndpointAddress(drsOptions.Value.APIAddress))
        {
            this.Endpoint.EndpointBehaviors.Add(new MyFaultLogger());
        }
    }

    // ReSharper disable once InconsistentNaming
    public partial class BookingCode : entity
    {
        public BookingCode()
        {
        }
    }

    class MyFaultLogger : IEndpointBehavior, IClientMessageInspector
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.IsFault)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Fault received!: {0}", reply);
                Console.ResetColor();
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }
    }
}
