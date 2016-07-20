using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IMessageSendingMediator
    {
        Task<TResult> SendAsync<TMessage, TResult>(MessageContext<TMessage> context) where TMessage : IMessage;
    }

    public static class MessageSendingMediatorExtensions
    {
        public static Task SendAsync<TMessage>(this IMessageSendingMediator mediator, MessageContext<TMessage> context)
            where TMessage : IMessage
        {
            Check.NotNull(mediator, nameof(mediator));

            return mediator.SendAsync<TMessage, VoidType>(context);
        }
    }
}