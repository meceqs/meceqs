using System;
using System.Collections.Generic;

namespace Meceqs.Sending
{
    public interface IMessageSender
    {
        ISendBuilder ForMessage(IMessage message);

        ISendBuilder ForMessage(IMessage message, Guid messageId);

        ISendBuilder ForMessages(IList<IMessage> messages);
    }
}