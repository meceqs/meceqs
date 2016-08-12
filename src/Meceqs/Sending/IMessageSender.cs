using System;
using System.Collections.Generic;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        IFluentSender ForMessage<TMessage>(TMessage message) where TMessage : class;

        IFluentSender ForMessage<TMessage>(TMessage message, Guid messageId) where TMessage : class;

        IFluentSender ForMessages<TMessage>(IList<TMessage> messages) where TMessage : class;
    }
}