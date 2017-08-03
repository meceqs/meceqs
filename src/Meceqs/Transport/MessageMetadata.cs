using System;

namespace Meceqs.Transport
{
    public class MessageMetadata
    {
        public Type MessageType { get; }
        public Type ResultType { get; }

        public MessageMetadata(Type messageType, Type resultType)
        {
            Guard.NotNull(messageType, nameof(messageType));
            // resultType may be null!

            MessageType = messageType;
            ResultType = resultType;
        }
    }
}