using System;

namespace Meceqs.Sending
{
    public class DefaultMessageSender : IMessageSender
    {
        private readonly IEnvelopeFactory _envelopeFactory;
        private readonly IMessageCorrelator _messageCorrelator;
        private readonly IMessageSendingMediator _sendingMediator;

        public DefaultMessageSender(
            IEnvelopeFactory envelopeFactory,
            IMessageCorrelator messageCorrelator,
            IMessageSendingMediator sendingMediator)
        {
            Check.NotNull(envelopeFactory, nameof(envelopeFactory));
            Check.NotNull(messageCorrelator, nameof(messageCorrelator));
            Check.NotNull(sendingMediator, nameof(sendingMediator));

            _envelopeFactory = envelopeFactory;
            _messageCorrelator = messageCorrelator;
            _sendingMediator = sendingMediator;
        }

        public ISendBuilder<TMessage> ForMessage<TMessage>(TMessage message, Guid messageId)
            where TMessage : IMessage
        {
            var envelope = _envelopeFactory.Create<TMessage>(message, messageId, new MessageHeaders());

            return new DefaultSendBuilder<TMessage>(envelope, _messageCorrelator, _sendingMediator);
        }
    }
}