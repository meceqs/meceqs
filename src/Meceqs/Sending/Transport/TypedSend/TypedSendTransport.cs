using System;
using System.Threading.Tasks;

namespace Meceqs.Sending.Transport.TypedSend
{
    public class TypedSendTransport : ISendTransport
    {
        private readonly IServiceProvider _serviceProvider;

        public TypedSendTransport(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public Task<TResult> SendAsync<TMessage, TResult>(SendContext<TMessage> context) where TMessage : IMessage
        {
            var sendHandler = (ISends<TMessage, TResult>)_serviceProvider.GetService(typeof(ISends<TMessage, TResult>));
            if (sendHandler == null)
            {
                throw new InvalidOperationException($"No implementation found for '{typeof(TMessage)}/{typeof(TResult)}'");
            }

            return sendHandler.SendAsync(context);
        }
    }
}