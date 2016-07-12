using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Sending;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class MessageSenderTest
    {
        private IMessageSender GetSender(IMessageCorrelator messageCorrelator = null, IMessageSendingMediator sendingMediator = null)
        {
            messageCorrelator = messageCorrelator ?? new DefaultMessageCorrelator();
            sendingMediator = sendingMediator ?? Substitute.For<IMessageSendingMediator>();

            return new DefaultMessageSender(TestObjects.EnvelopeFactory(), messageCorrelator, sendingMediator);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            var envelopeFactory = Substitute.For<IEnvelopeFactory>();
            var correlator = Substitute.For<IMessageCorrelator>();
            var sendingMediator = Substitute.For<IMessageSendingMediator>();

            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(null, correlator, sendingMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(envelopeFactory, null, sendingMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(envelopeFactory, correlator, null));
        }

        [Fact]
        public async Task Calls_Transport()
        {
            // Arrange
            var sourceCmd = TestObjects.Envelope<SimpleCommand>();
            var resultEvent = new SimpleEvent();

            var sendingMediator = Substitute.For<IMessageSendingMediator>();
            var sender = GetSender(sendingMediator: sendingMediator);

            // Act
            string result = await sender.ForEvent(resultEvent, Guid.NewGuid(), sourceCmd)
                .SendAsync<string>();

            // Assert
            await sendingMediator.ReceivedWithAnyArgs(1).SendAsync<SimpleEvent, string>(Arg.Any<MessageContext<SimpleEvent>>());
        }

        [Fact]
        public async Task Saves_settings_in_SendContext()
        {
            // Arrange

            var correlationId = Guid.NewGuid();
            var sourceCmd = TestObjects.Envelope<SimpleCommand>();
            sourceCmd.CorrelationId = correlationId;

            var resultEvent = new SimpleEvent();
            var resultEventId = Guid.NewGuid();

            var sendingMediator = Substitute.For<IMessageSendingMediator>();

            var sender = GetSender(sendingMediator: sendingMediator);
            var cancellationSource = new CancellationTokenSource();

            MessageContext<SimpleEvent> sendContext = null;
            await sendingMediator.SendAsync<SimpleEvent, string>(Arg.Do<MessageContext<SimpleEvent>>(x => sendContext = x));

            // Act

            string result = await sender.ForEvent(resultEvent, resultEventId, sourceCmd)
                .SetCancellationToken(cancellationSource.Token)
                .SetHeader("Key", "Value")
                .SetContextItem("SendKey", "SendValue")
                .SendAsync<string>();

            // Assert

            await sendingMediator.ReceivedWithAnyArgs(1).SendAsync<SimpleEvent, string>(Arg.Any<MessageContext<SimpleEvent>>());

            Assert.NotNull(sendContext);
            Assert.Equal(resultEventId, sendContext.Envelope.MessageId);
            Assert.Equal(resultEvent, sendContext.Envelope.Message);
            Assert.Equal(cancellationSource.Token, sendContext.Cancellation);
            Assert.Equal("Value", sendContext.Envelope.Headers["Key"]);
            Assert.Equal("SendValue", sendContext.GetContextItem<string>("SendKey"));
        }
    }
}