namespace Meceqs.Sending.Transport
{
    public interface ISendTransportFactory
    {
        ISendTransport CreateSendTransport<TMessage>(MessageContext<TMessage> context) where TMessage : IMessage;
    }
}