using Meceqs.Sending.Transport.TypedSend;

namespace Meceqs.Sending.Transport
{
    public class DefaultSendTransportFactory : ISendTransportFactory
    {
        private readonly ISenderFactory _senderFactory;

        public DefaultSendTransportFactory(ISenderFactory senderFactory)
        {
            Check.NotNull(senderFactory, nameof(senderFactory));

            _senderFactory = senderFactory;
        }

        public ISendTransport CreateSendTransport<TMessage>(MessageContext<TMessage> context) where TMessage : IMessage
        {
            return new TypedSendTransport(_senderFactory);
        }
    }
}