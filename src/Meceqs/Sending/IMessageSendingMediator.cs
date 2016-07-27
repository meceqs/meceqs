using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IMessageSendingMediator
    {
        Task<TResult> SendAsync<TResult>(MessageContext context);
    }

    public static class MessageSendingMediatorExtensions
    {
        public static Task SendAsync(this IMessageSendingMediator mediator, MessageContext context)
        {
            Check.NotNull(mediator, nameof(mediator));

            return mediator.SendAsync<VoidType>(context);
        }
    }
}