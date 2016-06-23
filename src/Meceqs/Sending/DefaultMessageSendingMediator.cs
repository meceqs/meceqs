using System;
using System.Threading.Tasks;
using Meceqs.Sending.Transport;

namespace Meceqs.Sending
{
    public class DefaultMessageSendingMediator : IMessageSendingMediator
    {
        private readonly ISendTransportResolver _transportResolver;
        private readonly ISendTransportInvoker _transportInvoker;

        public DefaultMessageSendingMediator(IServiceProvider serviceProvider)
            : this(new DefaultSendTransportResolver(serviceProvider), new DefaultSendTransportInvoker())
        {
        }

        public DefaultMessageSendingMediator(ISendTransportResolver transportResolver, ISendTransportInvoker transportInvoker)
        {
            if (transportResolver == null)
                throw new ArgumentNullException(nameof(transportResolver));

            if (transportInvoker == null)
                throw new ArgumentNullException(nameof(transportInvoker));

            _transportResolver = transportResolver;
            _transportInvoker = transportInvoker;
        }

        public async Task<TResult> SendAsync<TMessage, TResult>(SendContext<TMessage> context) where TMessage : IMessage
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // This doesn't use the ServiceProvider directly because we want to give people the opportunity
            // to resolve the transport based on the context (e.g. based on a ContextData-value).
            var transport = _transportResolver.Resolve<TMessage>(context);
            if (transport == null)
            {
                throw new InvalidOperationException($"No transport found for '{typeof(TMessage)}'");
            }

            // Having another component which actually calls the transport
            // allows people to use decorators that already know about the correct transport.
            var result = await _transportInvoker.InvokeSendAsync<TMessage, TResult>(transport, context);

            return result;
        }
    }
}