using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface ISendBuilder<TMessage> where TMessage : IMessage
    {
        ISendBuilder<TMessage> CorrelateWith(Envelope source);

        ISendBuilder<TMessage> SetCancellationToken(CancellationToken cancellation);

        ISendBuilder<TMessage> SetHeader(string headerName, object value);

        ISendBuilder<TMessage> SetContextItem(string key, object value);

        MessageContext<TMessage> BuildContext();

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}