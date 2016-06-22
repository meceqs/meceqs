using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Sending;
using Meceqs.Sending.Transport;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class MessageSenderTest
    {
        private IMessageSender GetSender(IMessageCorrelator messageCorrelator = null, ISendTransportMediator transportMediator = null)
        {
            messageCorrelator = messageCorrelator ?? new DefaultMessageCorrelator();
            transportMediator = transportMediator ?? Substitute.For<ISendTransportMediator>();

            return new DefaultMessageSender(new DefaultEnvelopeFactory(), messageCorrelator, transportMediator);
        }

        private Envelope<TMessage> GetEnvelope<TMessage>(TMessage message = null, Guid? id = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return new DefaultEnvelopeFactory().Create(message, id ?? Guid.NewGuid());
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            var envelopeFactory = Substitute.For<IEnvelopeFactory>();
            var correlator = Substitute.For<IMessageCorrelator>();
            var transportMediator = Substitute.For<ISendTransportMediator>();

            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(null, correlator, transportMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(envelopeFactory, null, transportMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(envelopeFactory, correlator, null));
        }

        [Fact]
        public async Task Calls_Transport()
        {
            // Arrange
            var sourceCmd = GetEnvelope<SimpleCommand>();
            var resultEvent = new SimpleEvent();

            var transportMediator = Substitute.For<ISendTransportMediator>();
            var sender = GetSender(transportMediator: transportMediator);

            // Act
            string result = await sender.ForEvent(resultEvent, Guid.NewGuid(), sourceCmd)
                .SendAsync<string>();

            // Assert
            await transportMediator.ReceivedWithAnyArgs(1).SendAsync<SimpleEvent, string>(Arg.Any<SendContext<SimpleEvent>>());
        }

        [Fact]
        public async Task Saves_settings_in_SendContext()
        {
            // Arrange

            var correlationId = Guid.NewGuid();
            var sourceCmd = GetEnvelope<SimpleCommand>();
            sourceCmd.CorrelationId = correlationId;

            var resultEvent = new SimpleEvent();
            var resultEventId = Guid.NewGuid();

            var transportMediator = Substitute.For<ISendTransportMediator>();

            var sender = GetSender(transportMediator: transportMediator);
            var cancellationSource = new CancellationTokenSource();

            SendContext<SimpleEvent> sendContext = null;
            await transportMediator.SendAsync<SimpleEvent, string>(Arg.Do<SendContext<SimpleEvent>>(x => sendContext = x));

            // Act

            string result = await sender.ForEvent(resultEvent, resultEventId, sourceCmd)
                .SetCancellationToken(cancellationSource.Token)
                .SetHeader("Key", "Value")
                .SetContextItem("SendKey", "SendValue")
                .SendAsync<string>();

            // Assert

            await transportMediator.ReceivedWithAnyArgs(1).SendAsync<SimpleEvent, string>(Arg.Any<SendContext<SimpleEvent>>());

            Assert.NotNull(sendContext);
            Assert.Equal(resultEventId, sendContext.Envelope.MessageId);
            Assert.Equal(resultEvent, sendContext.Envelope.Message);
            Assert.Equal(cancellationSource.Token, sendContext.Cancellation);
            Assert.Equal("Value", sendContext.Envelope.Headers["Key"]);
            Assert.Equal("SendValue", sendContext.GetContextItem<string>("SendKey"));
        }
    }
}