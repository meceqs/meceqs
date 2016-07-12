using System.Threading.Tasks;

namespace Meceqs.Sending.Transport
{
    public class DefaultSendTransportInvoker : ISendTransportInvoker
    {
        public Task<TResult> InvokeSendAsync<TMessage, TResult>(ISendTransport transport, MessageContext<TMessage> context)
            where TMessage : IMessage
        {
            Check.NotNull(transport, nameof(transport));

            return transport.SendAsync<TMessage, TResult>(context);
        }
    }
}