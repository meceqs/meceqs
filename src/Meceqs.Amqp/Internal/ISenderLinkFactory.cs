using Amqp;

namespace Meceqs.Amqp.Internal
{
    public interface ISenderLinkFactory
    {
        ISenderLink CreateSenderLink(Address address, string senderLinkName, string senderLinkAddress);
    }
}