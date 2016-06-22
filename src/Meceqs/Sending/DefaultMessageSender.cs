using System;
using Meceqs.Sending.TypedSend;

namespace Meceqs.Sending
{
    public class DefaultMessageSender : IMessageSender
    {
        private readonly ISendTransport _sendTransport;
        private readonly IMessageCorrelator _messageCorrelator;

        public DefaultMessageSender(IServiceProvider serviceProvider)
            : this(new TypedSendTransport(serviceProvider), new DefaultMessageCorrelator())
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
            var envelope = new MessageEnvelope<TMessage>(message, messageId);

            return new DefaultSendBuilder<TMessage>(envelope, _messageCorrelator)
                .UseTransport(_sendTransport);
        }
    }
}