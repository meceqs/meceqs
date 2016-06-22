using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IMessageEnvelopeSender<TMessage> where TMessage : IMessage
    {
        IMessageEnvelopeSender<TMessage> CorrelateWith(MessageEnvelope source);

        IMessageEnvelopeSender<TMessage> SetCancellationToken(CancellationToken cancellation);

        IMessageEnvelopeSender<TMessage> SetHeader(string headerName, object value);

        IMessageEnvelopeSender<TMessage> SetSendProperty(string key, object value);

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}