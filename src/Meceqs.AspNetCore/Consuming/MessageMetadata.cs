using System;

namespace Meceqs.AspNetCore.Consuming
{
    public class MessageMetadata
    {
        public Type MessageType { get; }
        public Type ResultType { get; }

        public MessageMetadata(Type messageType, Type resultType)
        {
            Check.NotNull(messageType, nameof(messageType));
            // resultType may be null!

            MessageType = messageType;
            ResultType = resultType;
        }
    }
}