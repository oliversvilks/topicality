using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.Xml;

namespace Topicality.Client.Application
{
    internal class CustomAddressHeader : MessageHeader
    {
        private readonly string name;
        private readonly string value;
        private readonly string addressNs;

        public CustomAddressHeader(string name, string addressNs, string value)
        {
            this.name = name;
            this.value = value;
            this.addressNs = addressNs;
        }

        public override string Name => name;

        public override string Namespace => addressNs;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("Address", addressNs);
            writer.WriteString(value);
            writer.WriteEndElement();
        }
    }
}
