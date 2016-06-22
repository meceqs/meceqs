using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Consuming
{
    public interface IMessageConsumerMediator
    {
        Task<TResult> SendAsync<TMessage, TResult>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage;
    }

    public static class MessageConsumerMediatorExtensions
    {
        public static async Task SendAsync<TMessage>(
            this IMessageConsumerMediator mediator,
            MessageEnvelope<TMessage> envelope,
            CancellationToken cancellation) where TMessage : IMessage
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            await mediator.SendAsync<TMessage, VoidType>(envelope, cancellation);
        }
    }
}