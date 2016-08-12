namespace Meceqs.Filters.TypedHandling
{
    public interface IHandlerFactory
    {
        IHandles<TMessage> CreateHandler<TMessage>()
            where TMessage : class;

        IHandles<TMessage, TResult> CreateHandler<TMessage, TResult>()
            where TMessage : class;
    }
}