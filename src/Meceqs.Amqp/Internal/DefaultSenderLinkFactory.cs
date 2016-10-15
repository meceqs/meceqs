using Amqp;

namespace Meceqs.Amqp.Internal
{
    public class DefaultSenderLinkFactory : ISenderLinkFactory
    {
        public ISenderLink CreateSenderLink(Address address, string senderLinkName, string senderLinkAddress)
        {
            Check.NotNull(address, nameof(address));
            Check.NotNullOrWhiteSpace(senderLinkName, nameof(senderLinkName));
            Check.NotNullOrWhiteSpace(senderLinkAddress, nameof(senderLinkAddress));

            var connection = new Connection(address);
            var session = new Session(connection);
            var senderLink = new SenderLink(session, senderLinkName, senderLinkAddress);

            return new SenderLinkWrapper(senderLink);
        }
    }
}