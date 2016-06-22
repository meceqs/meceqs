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
        private IMessageSender GetSender(ISendTransport sendTransport = null, IMessageCorrelator messageCorrelator = null)
        {
            sendTransport = sendTransport ?? Substitute.For<ISendTransport>();
            messageCorrelator = messageCorrelator ?? new DefaultMessageCorrelator();

            return new DefaultMessageSender(sendTransport, messageCorrelator);
        }

        private MessageEnvelope<TMessage> GetEnvelope<TMessage>(TMessage message = null, Guid? id = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return new MessageEnvelope<TMessage>(message, id ?? Guid.NewGuid());
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(null, Substitute.For<IMessageCorrelator>()));
            Assert.Throws<ArgumentNullException>(() => new DefaultMessageSender(Substitute.For<ISendTransport>(), null));
        }

        [Fact]
        public async Task Calls_Transport()
        {
            // Arrange
            var sourceCmd = GetEnvelope<SimpleCommand>();
            var resultEvent = new SimpleEvent();

            var transport = Substitute.For<ISendTransport>();
            var sender = GetSender(transport);

            // Act
            string result = await sender.ForEvent(resultEvent, Guid.NewGuid(), sourceCmd)
                .SendAsync<string>();

            // Assert
            await transport.ReceivedWithAnyArgs(1).SendAsync<SimpleEvent, string>(Arg.Any<SendContext<SimpleEvent>>());
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

            var transport = Substitute.For<ISendTransport>();

            var sender = GetSender(transport);
            var cancellationSource = new CancellationTokenSource();

            SendContext<SimpleEvent> sendContext = null;
            await transport.SendAsync<SimpleEvent, string>(Arg.Do<SendContext<SimpleEvent>>(x => sendContext = x));

            // Act

            string result = await sender.ForEvent(resultEvent, resultEventId, sourceCmd)
                .SetCancellationToken(cancellationSource.Token)
                .SetHeader("Key", "Value")
                .SetSendProperty("SendKey", "SendValue")
                .SendAsync<string>();

            // Assert

            await transport.ReceivedWithAnyArgs(1).SendAsync<SimpleEvent, string>(Arg.Any<SendContext<SimpleEvent>>());

            Assert.NotNull(sendContext);
            Assert.Equal(resultEventId, sendContext.Envelope.MessageId);
            Assert.Equal(resultEvent, sendContext.Envelope.Message);
            Assert.Equal(cancellationSource.Token, sendContext.Cancellation);
            Assert.Equal("Value", sendContext.Envelope.Headers["Key"]);
            Assert.Equal("SendValue", sendContext.GetContextItem<string>("SendKey"));
        }
    }
}