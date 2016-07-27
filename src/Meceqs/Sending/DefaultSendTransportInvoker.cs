using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public class DefaultSendTransportInvoker : ISendTransportInvoker
    {
        public Task<TResult> InvokeSendAsync<TResult>(ISendTransport transport, MessageContext context)
        {
            Check.NotNull(transport, nameof(transport));

            return transport.SendAsync<TResult>(context);
        }
    }
}