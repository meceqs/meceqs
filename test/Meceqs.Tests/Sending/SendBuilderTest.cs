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
        private MessageEnvelope<TMessage> GetEnvelope<TMessage>(TMessage message = null, Guid? id = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return new MessageEnvelope<TMessage>(message, id ?? Guid.NewGuid());
        }

        private ISendBuilder<TMessage> GetBuilder<TMessage>(MessageEnvelope<TMessage> envelope = null, IMessageCorrelator correlator = null)
            where TMessage : class, IMessage, new()
        {
            envelope = envelope ?? GetEnvelope<TMessage>();

            correlator = correlator ?? new DefaultMessageCorrelator();
            return new DefaultSendBuilder<TMessage>(envelope, correlator);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            // Arrange
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder<SimpleMessage>(null, new DefaultMessageCorrelator()));
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder<SimpleMessage>(GetEnvelope<SimpleMessage>(), null));
        }

        [Fact]
        public async Task Throws_on_Send_if_Transport_is_missing()
        {
            // Arrange
            var builder = GetBuilder<SimpleMessage>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await builder.SendAsync());
        }

        [Fact]
        public async Task Uses_given_transport_for_Send()
        {
            // Arrange
            var builder = GetBuilder<SimpleMessage>();
            var transport = Substitute.For<ISendTransport>();

            // Act
            await builder.UseTransport(transport)
                .SendAsync();

            // Assert
            await transport.Received(1).SendAsync<SimpleMessage, VoidType>(Arg.Any<SendContext<SimpleMessage>>());
        }

        [Fact]
        public void Calls_Correlator()
        {
            // Arrange
            var envelope = GetEnvelope<SimpleMessage>();
            var correlator = Substitute.For<IMessageCorrelator>();
            var builder = GetBuilder<SimpleMessage>(envelope, correlator);

            // Act
            var sourceMsg = GetEnvelope<SimpleCommand>();
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
            builder.SetSendProperty("Key", "Value");
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
            var envelope = GetEnvelope<SimpleMessage>();
            var builder = GetBuilder<SimpleMessage>(envelope);

            // Act
            var context = builder.BuildSendContext();

            // Assert
            Assert.Equal(envelope, context.Envelope);
        }
    }
}