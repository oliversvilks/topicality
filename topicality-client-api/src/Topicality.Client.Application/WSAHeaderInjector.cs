using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Topicality.Client.Application
{
    public class WSAHeaderInjector : IEndpointBehavior, IClientMessageInspector
    {
        public const string addressNs = "http://www.w3.org/2005/08/addressing";
        private string destinationCountry;
        public WSAHeaderInjector()
        {
        }
        public WSAHeaderInjector(string destinationCountry)
        {
            this.destinationCountry = destinationCountry;
        }

        #region IEndpointBehavior
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
        #endregion

        #region IClientMessageInspector
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
           // request.Headers.Add(MessageHeader.CreateHeader("Action", addressNs, "CCN2.Service.Taxation.Default.AEOI_DAC7.InitialMessageBAS/send"));
            request.Headers.Add(new CustomAddressHeader("From", addressNs, "partner:CCN2.Partner.LV.Taxation.TAXUD/AEOI_DAC7.CONF"));
            if (!string.IsNullOrEmpty(destinationCountry))
            {
                //TODO noskaidrot kā veidojas patiesā "to" adrese
                //request.Headers.Add(new CustomAddressHeader("To", addressNs, $"partner:CCN2.Partner.{destinationCountry}.Taxation.TAXUD"));
            }
            //request.Headers.Add(MessageHeader.CreateHeader("MessageID", addressNs, Guid.NewGuid()));

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Remove "To" header from response to avoid System.ServiceModel.ProtocolException.
            //for (int i = 0; i < reply.Headers.Count; i++)
            //{
            //    if (reply.Headers[i].Name.Equals("To"))
            //    {
            //        reply.Headers.RemoveAt(i);
            //        break;
            //    }
            //}
        }
        #endregion
    }
}
