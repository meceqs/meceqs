using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Handling
{
    public interface IMessageHandlingMediator
    {
        Task<TResult> HandleAsync<TMessage, TResult>(MessageEnvelope<TMessage> envelope, CancellationToken cancellation)
            where TMessage : IMessage;
    }

    public static class MessageHandlingMediatorExtensions
    {
        public static async Task HandleAsync<TMessage>(
            this IMessageHandlingMediator mediator,
            MessageEnvelope<TMessage> envelope,
            CancellationToken cancellation) where TMessage : IMessage
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            await mediator.HandleAsync<TMessage, VoidType>(envelope, cancellation);
        }
    }
}