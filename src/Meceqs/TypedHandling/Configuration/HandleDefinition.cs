using System;

namespace Meceqs.TypedHandling.Configuration
{
    public struct HandleDefinition
    {
        public Type MessageType { get; }

        public Type ResultType { get; }

        public HandleDefinition(Type messageType, Type resultType)
        {
            Guard.NotNull(messageType, nameof(messageType));
            // Result type may be null!

            MessageType = messageType;
            ResultType = resultType;
        }
    }
}