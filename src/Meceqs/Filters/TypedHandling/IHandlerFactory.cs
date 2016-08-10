namespace Meceqs.Filters.TypedHandling
{
    public interface IHandlerFactory
    {
        IHandles<TMessage, TResult> CreateHandler<TMessage, TResult>()
            where TMessage : IMessage;
    }
}