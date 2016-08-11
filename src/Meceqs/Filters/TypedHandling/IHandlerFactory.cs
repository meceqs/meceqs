namespace Meceqs.Filters.TypedHandling
{
    public interface IHandlerFactory
    {
        IHandles<TMessage> CreateHandler<TMessage>()
            where TMessage : IMessage;

        IHandles<TMessage, TResult> CreateHandler<TMessage, TResult>()
            where TMessage : IMessage;
    }
}