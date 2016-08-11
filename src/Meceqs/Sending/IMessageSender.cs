using System;
using System.Collections.Generic;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        IFluentSender ForMessage(IMessage message);

        IFluentSender ForMessage(IMessage message, Guid messageId);

        IFluentSender ForMessages(IList<IMessage> messages);
    }
}