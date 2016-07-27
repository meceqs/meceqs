namespace Meceqs.Sending.TypedSend
{
    public interface ISenderFactory
    {
        ISender<TMessage, TResult> CreateSender<TMessage, TResult>()
            where TMessage : IMessage;
    }
}