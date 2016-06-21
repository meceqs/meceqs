namespace Meceqs.Consuming
{
    public interface IConsumerResolver
    {
        IConsumes<TMessage, TResult> Resolve<TMessage, TResult>() where TMessage : IMessage;
    }
}