using System;

namespace Meceqs.TypedHandling.Configuration
{
    public class HandleDefinition : IEquatable<HandleDefinition>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as HandleDefinition);
        }

        public bool Equals(HandleDefinition other)
        {
            return other != null
                && MessageType == other.MessageType
                && ResponseType == other.ResponseType;
        }

        public override int GetHashCode()
        {
            var hashCode = 352033288;
            hashCode = hashCode * -1521134295 + MessageType.GetHashCode();
            hashCode = hashCode * -1521134295 + ResponseType.GetHashCode();
            return hashCode;
        }
    }
}
