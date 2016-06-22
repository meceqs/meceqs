using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface ISendBuilder<TMessage> where TMessage : IMessage
    {
        ISendBuilder<TMessage> UseTransport(ISendTransport sendTransport);

        ISendBuilder<TMessage> CorrelateWith(IMessageEnvelope source);

        ISendBuilder<TMessage> SetCancellationToken(CancellationToken cancellation);

        ISendBuilder<TMessage> SetHeader(string headerName, object value);

        ISendBuilder<TMessage> SetSendProperty(string key, object value);

        SendContext<TMessage> BuildSendContext();

        Task SendAsync();

        Task<TResult> SendAsync<TResult>();
    }
}