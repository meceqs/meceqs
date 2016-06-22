using System;

namespace Meceqs.Sending
{
    public class DefaultMessageSender : IMessageSender
    {
        private readonly ISendTransport _sendTransport;
        private readonly IMessageCorrelator _messageCorrelator;

        public DefaultMessageSender(ISendTransport sendTransport)
            : this(sendTransport, new DefaultMessageCorrelator())
        {
        }

        public DefaultMessageSender(ISendTransport sendTransport, IMessageCorrelator messageCorrelator)
        {
            if (sendTransport == null)
                throw new ArgumentNullException(nameof(sendTransport));

            if (messageCorrelator == null)
                throw new ArgumentNullException(nameof(messageCorrelator));

            _sendTransport = sendTransport;
            _messageCorrelator = messageCorrelator;
        }

        public ISendBuilder<TMessage> ForMessage<TMessage>(TMessage message, Guid messageId)
            where TMessage : IMessage
        {
            var envelope = new MessageEnvelope<TMessage>(messageId, message);

            return new DefaultSendBuilder<TMessage>(envelope, _messageCorrelator)
                .UseTransport(_sendTransport);
        }
    }
}