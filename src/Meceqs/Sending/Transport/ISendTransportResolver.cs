namespace Meceqs.Sending.Transport
{
    public interface ISendTransportResolver
    {
        ISendTransport Resolve<TMessage>(SendContext<TMessage> context) where TMessage : IMessage;
    }
}