namespace Meceqs.Sending
{
    public interface ISendTransportFactory
    {
        ISendTransport CreateSendTransport(MessageContext context);
    }
}