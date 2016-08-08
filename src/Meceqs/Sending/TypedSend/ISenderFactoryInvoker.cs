using System;

namespace Meceqs.Sending.TypedSend
{
    public interface ISenderFactoryInvoker
    {
        object InvokeCreateSender(ISenderFactory senderFactory, Type messageType, Type resultType);
    }
}