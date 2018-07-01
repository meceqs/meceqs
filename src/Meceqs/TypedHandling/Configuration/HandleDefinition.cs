using System;

namespace Meceqs.TypedHandling.Configuration
{
    public struct HandleDefinition
    {
        public Type MessageType { get; }

        public Type ResponseType { get; }

        public HandleDefinition(Type messageType, Type responseType)
        {
            Guard.NotNull(messageType, nameof(messageType));
            Guard.NotNull(responseType, nameof(responseType));

            MessageType = messageType;
            ResponseType = responseType;
        }
    }
}