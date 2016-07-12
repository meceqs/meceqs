namespace Meceqs.Handling
{
    public interface IHandlerFactory
    {
        IHandler<TMessage, TResult> CreateHandler<TMessage, TResult>()
            where TMessage : IMessage;
    }
}