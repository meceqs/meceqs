using System;
using System.Collections.Generic;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        IFluentSender ForMessage(object message, Guid? messageId = null);

        IFluentSender ForMessages<TMessage>(IList<TMessage> messages) where TMessage : class;
    }
}