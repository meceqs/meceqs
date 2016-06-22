using System;
using Meceqs.Sending.Transport;

namespace Meceqs.Sending
{
    public class DefaultMessageSender : IMessageSender
    {
        private readonly IEnvelopeFactory _envelopeFactory;
        private readonly IMessageCorrelator _messageCorrelator;
        private readonly ISendTransportMediator _transportMediator;

        public DefaultMessageSender(IServiceProvider serviceProvider)
            : this(new DefaultEnvelopeFactory(), new DefaultMessageCorrelator(), new DefaultSendTransportMediator(serviceProvider))
        {
        }

        public DefaultMessageSender(
            IEnvelopeFactory envelopeFactory,
            IMessageCorrelator messageCorrelator,
            ISendTransportMediator transportMediator)
        {
            if (envelopeFactory == null)
                throw new ArgumentNullException(nameof(envelopeFactory));

            if (messageCorrelator == null)
                throw new ArgumentNullException(nameof(messageCorrelator));

            if (transportMediator == null)
                throw new ArgumentNullException(nameof(transportMediator));

            _envelopeFactory = envelopeFactory;
            _transportMediator = transportMediator;
            _messageCorrelator = messageCorrelator;
        }

        public ISendBuilder<TMessage> ForMessage<TMessage>(TMessage message, Guid messageId)
            where TMessage : IMessage
        {
            var envelope = _envelopeFactory.Create<TMessage>(message, messageId, new MessageHeaders());

            return new DefaultSendBuilder<TMessage>(envelope, _messageCorrelator, _transportMediator);
        }
    }
}