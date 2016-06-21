using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        Task<TResult> SendAsync<TMessage, TResult>(Guid messageId, TMessage msg, IMessage sourceMessage, SendProperties sendProperties, CancellationToken cancellation);

        // Task<TResult> SendAsync<TMessage, TResult>(MessageEnvelope<TMessage> envelope, SendProperties sendProperties, CancellationToken cancellation)
        //     where TMessage : IMessage;
    }
}