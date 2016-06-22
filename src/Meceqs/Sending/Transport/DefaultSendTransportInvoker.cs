using System;
using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public class DefaultSendTransportInvoker : ISendTransportInvoker
    {
        public async Task<TResult> InvokeSendAsync<TMessage, TResult>(ISendTransport transport, SendContext<TMessage> context)
            where TMessage : IMessage
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));

            return await transport.SendAsync<TMessage, TResult>(context);
        }
    }
}