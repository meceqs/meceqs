namespace Meceqs.Handling
{
    public interface IHandlerResolver
    {
        IHandles<TMessage, TResult> Resolve<TMessage, TResult>() where TMessage : IMessage;
    }
}