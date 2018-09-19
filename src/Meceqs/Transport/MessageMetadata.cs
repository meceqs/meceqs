using System;

namespace Meceqs.Transport
{
    public class MessageMetadata
    {
        public Type MessageType { get; }
        public Type ResponseType { get; }

        public MessageMetadata(Type messageType, Type responseType)
        {
            Guard.NotNull(messageType, nameof(messageType));
            // responseType may be null!

            MessageType = messageType;
            ResponseType = responseType;
        }
    }
}
