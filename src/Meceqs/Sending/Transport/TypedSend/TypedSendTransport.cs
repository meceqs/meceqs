using System;
using System.Threading.Tasks;

namespace Meceqs.Sending.Transport.TypedSend
{
    public class TypedSendTransport : ISendTransport
    {
        private readonly ISenderFactory _senderFactory;

        public TypedSendTransport(ISenderFactory senderFactory)
        {
            Check.NotNull(senderFactory, nameof(senderFactory));

            _senderFactory = senderFactory;
        }

        public Task<TResult> SendAsync<TMessage, TResult>(MessageContext<TMessage> context) where TMessage : IMessage
        {
            var sender = _senderFactory.CreateSender<TMessage, TResult>();
            if (sender == null)
            {
                throw new InvalidOperationException($"No sender found for '{typeof(TMessage)}/{typeof(TResult)}'");
            }

            return sender.SendAsync(context);
        }
    }
}