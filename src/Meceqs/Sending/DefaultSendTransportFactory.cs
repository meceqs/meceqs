using Meceqs.Sending.TypedSend;

namespace Meceqs.Sending
{
    public class DefaultSendTransportFactory : ISendTransportFactory
    {
        private readonly ISenderFactory _senderFactory;
        private readonly ISenderFactoryInvoker _senderFactoryInvoker;
        private readonly ISenderInvoker _senderInvoker;

        public DefaultSendTransportFactory(ISenderFactory senderFactory, ISenderFactoryInvoker senderFactoryInvoker, ISenderInvoker senderInvoker)
        {
            Check.NotNull(senderFactory, nameof(senderFactory));
            Check.NotNull(senderFactoryInvoker, nameof(senderFactoryInvoker));
            Check.NotNull(senderInvoker, nameof(senderInvoker));

            _senderFactory = senderFactory;
            _senderFactoryInvoker = senderFactoryInvoker;
            _senderInvoker = senderInvoker;
        }

        public ISendTransport CreateSendTransport(MessageContext context)
        {
            return new TypedSendTransport(_senderFactory, _senderFactoryInvoker, _senderInvoker);
        }
    }
}