using System;

namespace Meceqs.TypedHandling.Configuration
{
    public struct HandleDefinition
    {
        public Type MessageType { get; }

        public Type ResultType { get; }

        public HandleDefinition(Type messageType, Type resultType)
        {
            Check.NotNull(messageType, nameof(messageType));

            MessageType = messageType;
            ResultType = resultType;
        }
    }
}