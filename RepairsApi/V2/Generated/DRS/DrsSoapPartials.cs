using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.Options;
using RepairsApi.V2.Services;

// ReSharper disable All

namespace V2_Generated_DRS
{
    public partial class SOAPClient
    {
        public SOAPClient(IOptions<DrsOptions> drsOptions) :
            this(new BasicHttpsBinding
            {
                MaxReceivedMessageSize = 262144
            }, new EndpointAddress(drsOptions.Value.APIAddress))
        {
            this.Endpoint.EndpointBehaviors.Add(new MyFaultLogger());
        }
    }

    public partial class bookingCode : entity
    {
        public bookingCode()
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
