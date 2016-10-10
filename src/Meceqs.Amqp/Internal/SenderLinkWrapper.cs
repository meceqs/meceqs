using System.Threading.Tasks;
using Amqp;

namespace Meceqs.Amqp.Internal
{
    public class SenderLinkWrapper : ISenderLink
    {
        private readonly SenderLink _senderLink;

        public SenderLinkWrapper(SenderLink senderLink)
        {
            Check.NotNull(senderLink, nameof(senderLink));

            _senderLink = senderLink;
        }

        public Task SendAsync(Message message)
        {
            return _senderLink.SendAsync(message);
        }

         public void Dispose()
        {
            _senderLink?.Close();
            _senderLink?.Session?.Close();
            _senderLink?.Session?.Connection?.Close();
        }
    }
}