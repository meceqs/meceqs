using Amqp;
using Meceqs.Transport;

namespace Meceqs.Amqp.Configuration
{
    public class AmqpSenderOptions : TransportSenderOptions
    {
        public Address Address { get; set; }

        public string SenderLinkName { get; set; }

        public string SenderLinkAddress { get; set; }
    }
}