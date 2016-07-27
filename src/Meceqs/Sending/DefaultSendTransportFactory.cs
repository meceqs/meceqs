using Meceqs.Sending.TypedSend;

namespace Meceqs.Sending
{
    public class DefaultSendTransportFactory : ISendTransportFactory
    {
        private readonly ITypedSendInvoker _typedSendInvoker;
        private readonly ISenderFactory _senderFactory;

        public DefaultSendTransportFactory(ITypedSendInvoker typedSendInvoker, ISenderFactory senderFactory)
        {
            Check.NotNull(typedSendInvoker, nameof(typedSendInvoker));
            Check.NotNull(senderFactory, nameof(senderFactory));

            _typedSendInvoker = typedSendInvoker;
            _senderFactory = senderFactory;
        }

        public ISendTransport CreateSendTransport(MessageContext context)
        {
            return new TypedSendTransport(_typedSendInvoker, _senderFactory);
        }
    }
}