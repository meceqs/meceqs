using System;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IMessageSendingMediator
    {
        Task<TResult> SendAsync<TMessage, TResult>(SendContext<TMessage> context) where TMessage : IMessage;
    }

    public static class MessageSendingMediatorExtensions
    {
        public static Task SendAsync<TMessage>(this IMessageSendingMediator mediator, SendContext<TMessage> context)
            where TMessage : IMessage
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            return mediator.SendAsync<TMessage, VoidType>(context);
        }
    }
}