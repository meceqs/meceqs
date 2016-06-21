using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IMessageConsumer
    {
        Task<TResult> ConsumeAsync<TMessage, TResult>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage;
    }

    public static class MessageConsumerExtensions
    {
        public static async Task ConsumeAsync<TMessage>(this IMessageConsumer messageConsumer, MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage
        {
            if (messageConsumer == null)
                throw new ArgumentNullException(nameof(messageConsumer));

            await messageConsumer.ConsumeAsync<TMessage, VoidType>(envelope, cancellation);
        }
    }
}