using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IMessageClient
    {
        ISendContextBuilder CreateMessage(Guid messageId, IMessage message, CancellationToken cancellation);

        Task<TResult> SendAsync<TResult>(SendContext context);
    }
}