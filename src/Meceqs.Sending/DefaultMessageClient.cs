using System;

namespace Meceqs.Sending
{
    public class DefaultMessageClient : IMessageClient
    {
        private readonly ISendTransport _sendTransport;
        private readonly IMessageCorrelator _messageCorrelator;

        public DefaultMessageClient(ISendTransport sendContextSender)
            : this(sendContextSender, new DefaultMessageCorrelator())
        {
        }

        public DefaultMessageClient(ISendTransport sendTransport, IMessageCorrelator messageCorrelator)
        {
            if (sendTransport == null)
                throw new ArgumentNullException(nameof(sendTransport));

            if (messageCorrelator == null)
                throw new ArgumentNullException(nameof(messageCorrelator));

            _sendTransport = sendTransport;
            _messageCorrelator = messageCorrelator;
        }

        public IMessageEnvelopeSender<TMessage> ForMessage<TMessage>(Guid messageId, TMessage message)
            where TMessage : IMessage
        {
            var envelope = new MessageEnvelope<TMessage>(messageId, message);

            return new DefaultMessageEnvelopeSender<TMessage>(_sendTransport, _messageCorrelator, envelope);
        }
    }
}