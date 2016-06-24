using System;
using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public class DefaultSendTransportInvoker : ISendTransportInvoker
    {
        public Task<TResult> InvokeSendAsync<TMessage, TResult>(ISendTransport transport, SendContext<TMessage> context)
            where TMessage : IMessage
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));

            return transport.SendAsync<TMessage, TResult>(context);
        }
    }
}