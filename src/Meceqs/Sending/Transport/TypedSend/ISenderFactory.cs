namespace Meceqs.Sending.Transport.TypedSend
{
    public interface ISenderFactory
    {
        ISender<TMessage, TResult> CreateSender<TMessage, TResult>()
            where TMessage : IMessage;
    }
}