using System;
using Meceqs.Sending.Transport.TypedSend;

namespace Meceqs.Sending.Transport
{
    public class DefaultSendTransportResolver : ISendTransportResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultSendTransportResolver(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public ISendTransport Resolve<TMessage>(SendContext<TMessage> context) where TMessage : IMessage
        {
            return new TypedSendTransport(_serviceProvider);
        }
    }
}