using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        IFluentSender ForMessage(object message);

        IFluentSender ForMessage(object message, Guid messageId);

        IFluentSender ForMessages<TMessage>(IList<TMessage> messages) where TMessage : class;

        Task SendAsync(object message, Guid? messageId = null);

        Task<TResult> SendAsync<TResult>(object message, Guid? messageId = null);
    }
}