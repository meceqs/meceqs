using Meceqs.Transport;
using Meceqs.TypedHandling.Configuration;

namespace Meceqs.TypedHandling
{
    public class TypedHandlingOptions
    {
        public HandlerCollection Handlers { get; } = new HandlerCollection();

        public InterceptorCollection Interceptors { get; } = new InterceptorCollection();

        public UnknownMessageBehavior UnknownMessageBehavior { get; set; } = UnknownMessageBehavior.ThrowException;

        public void ThrowOnUnknownMessage()
        {
            UnknownMessageBehavior = UnknownMessageBehavior.ThrowException;
        }

        public void SkipUnknownMessages()
        {
            UnknownMessageBehavior = UnknownMessageBehavior.Skip;
        }
    }
}