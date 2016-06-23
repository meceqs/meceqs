using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Sending;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class SendBuilderTest
    {
        private ISendBuilder<TMessage> GetBuilder<TMessage>(
            Envelope<TMessage> envelope = null,
            IMessageCorrelator correlator = null,
            IMessageSendingMediator sendingMediator = null) where TMessage : class, IMessage, new()
        {
            envelope = envelope ?? TestObjects.Envelope<TMessage>();
            correlator = correlator ?? new DefaultMessageCorrelator();
            sendingMediator = sendingMediator ?? Substitute.For<IMessageSendingMediator>();

            return new DefaultSendBuilder<TMessage>(envelope, correlator, sendingMediator);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            // Arrange
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var correlator = new DefaultMessageCorrelator();
            var sendingMediator = Substitute.For<IMessageSendingMediator>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder<SimpleMessage>(null, correlator, sendingMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder<SimpleMessage>(envelope, null, sendingMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder<SimpleMessage>(envelope, correlator, null));
        }

        [Fact]
        public async Task Calls_TransportMediator()
        {
            // Arrange
            var sendingMediator = Substitute.For<IMessageSendingMediator>();
            var builder = GetBuilder<SimpleMessage>(sendingMediator: sendingMediator);

            // Act
            await builder.SendAsync();

            // Assert
            await sendingMediator.Received(1).SendAsync<SimpleMessage, VoidType>(Arg.Any<SendContext<SimpleMessage>>());
        }

        [Fact]
        public void Calls_Correlator()
        {
            // Arrange
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var correlator = Substitute.For<IMessageCorrelator>();
            var builder = GetBuilder<SimpleMessage>(envelope, correlator);

            // Act
            var sourceMsg = TestObjects.Envelope<SimpleCommand>();
            builder.CorrelateWith(sourceMsg);

            // Assert
            correlator.Received(1).CorrelateSourceWithTarget(sourceMsg, envelope);
        }

        [Fact]
        public void Saves_Header_In_Envelope()
        {
            // Arrange
            var builder = GetBuilder<SimpleMessage>();

            // Act
            builder.SetHeader("Key", "Value");
            var context = builder.BuildSendContext();

            // Assert
            Assert.Equal("Value", context.Envelope.Headers["Key"]);
        }

        [Fact]
        public void Saves_SendProperty_In_Context()
        {
            // Arrange
            var builder = GetBuilder<SimpleMessage>();

            // Act
            builder.SetContextItem("Key", "Value");
            var context = builder.BuildSendContext();

            // Assert
            Assert.Equal("Value", context.GetContextItem<string>("Key"));
        }

        [Fact]
        public void Saves_CancellationToken_In_Context()
        {
            // Arrange
            var builder = GetBuilder<SimpleMessage>();
            var cancellationSource = new CancellationTokenSource();

            // Act
            builder.SetCancellationToken(cancellationSource.Token);
            var context = builder.BuildSendContext();

            // Assert
            Assert.Equal(cancellationSource.Token, context.Cancellation);
        }

        [Fact]
        public void Saves_Envelope_In_Context()
        {
            // Arrange
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var builder = GetBuilder<SimpleMessage>(envelope);

            // Act
            var context = builder.BuildSendContext();

            // Assert
            Assert.Equal(envelope, context.Envelope);
        }
    }
}