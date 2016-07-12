using System;
using System.Threading.Tasks;
using Meceqs.Sending.Transport;

namespace Meceqs.Sending
{
    public class DefaultMessageSendingMediator : IMessageSendingMediator
    {
        private readonly ISendTransportFactory _transportFactory;
        private readonly ISendTransportInvoker _transportInvoker;

        public DefaultMessageSendingMediator(ISendTransportFactory transportFactory, ISendTransportInvoker transportInvoker)
        {
            Check.NotNull(transportFactory, nameof(transportFactory));
            Check.NotNull(transportInvoker, nameof(transportInvoker));

            _transportFactory = transportFactory;
            _transportInvoker = transportInvoker;
        }

        public Task<TResult> SendAsync<TMessage, TResult>(MessageContext<TMessage> context) where TMessage : IMessage
        {
            Check.NotNull(context, nameof(context));

            var transport = _transportFactory.CreateSendTransport<TMessage>(context);
            if (transport == null)
            {
                throw new InvalidOperationException($"No transport found for '{typeof(TMessage)}'");
            }

            // Having another component which actually calls the transport
            // allows people to use decorators that already know about the correct transport.
            return _transportInvoker.InvokeSendAsync<TMessage, TResult>(transport, context);
        }
    }
}