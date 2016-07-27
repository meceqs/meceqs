using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Sending;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class SendBuilderTest
    {
        private ISendBuilder GetBuilder<TMessage>(
            Envelope<TMessage> envelope = null,
            IEnvelopeCorrelator correlator = null,
            IMessageSendingMediator sendingMediator = null) where TMessage : class, IMessage, new()
        {
            envelope = envelope ?? TestObjects.Envelope<TMessage>();
            correlator = correlator ?? new DefaultEnvelopeCorrelator();
            sendingMediator = sendingMediator ?? Substitute.For<IMessageSendingMediator>();

            var envelopes = new List<Envelope> { envelope };

            return new DefaultSendBuilder(envelopes, correlator, new DefaultMessageContextFactory(), sendingMediator);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            // Arrange
            var envelopes = new List<Envelope>();
            var correlator = new DefaultEnvelopeCorrelator();
            var messageContextFactory = new DefaultMessageContextFactory();
            var sendingMediator = Substitute.For<IMessageSendingMediator>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder(null, correlator, messageContextFactory, sendingMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder(envelopes, null, messageContextFactory, sendingMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder(envelopes, correlator, null, sendingMediator));
            Assert.Throws<ArgumentNullException>(() => new DefaultSendBuilder(envelopes, correlator, messageContextFactory, null));
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
            await sendingMediator.Received(1).SendAsync<VoidType>(Arg.Any<MessageContext<SimpleMessage>>());
        }

        [Fact]
        public void Calls_Correlator()
        {
            // Arrange
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var correlator = Substitute.For<IEnvelopeCorrelator>();
            var builder = GetBuilder<SimpleMessage>(envelope, correlator);

            // Act
            var sourceMsg = TestObjects.Envelope<SimpleCommand>();
            builder.CorrelateWith(sourceMsg);

            // Assert
            correlator.Received(1).CorrelateSourceWithTarget(sourceMsg, envelope);
        }

        [Fact]
        public async Task Saves_Header_In_Envelope()
        {
            // Arrange

            int called = 0;
            var mediator = Substitute.For<IMessageSendingMediator>();
            mediator.When(x => x.SendAsync(Arg.Any<MessageContext>()))
                .Do(x => {
                    called++;

                    Assert.Equal("Value", x.Arg<MessageContext>().Envelope.Headers["Key"]);
                });

            var builder = GetBuilder<SimpleMessage>(sendingMediator: mediator);

            // Act
            builder.SetHeader("Key", "Value");
            await builder.SendAsync();

            // Assert
            Assert.Equal(1, called);
        }

        [Fact]
        public async Task Saves_SendProperty_In_Context()
        {
            // Arrange
            int called = 0;
            var mediator = Substitute.For<IMessageSendingMediator>();
            mediator.When(x => x.SendAsync(Arg.Any<MessageContext>()))
                .Do(x => {
                    called++;

                    Assert.Equal("Value", x.Arg<MessageContext>().GetContextItem<string>("Key"));
                });

            var builder = GetBuilder<SimpleMessage>(sendingMediator: mediator);

            // Act
            builder.SetContextItem("Key", "Value");
            await builder.SendAsync();

            // Assert
            Assert.Equal(1, called);
        }

        [Fact]
        public async Task Saves_CancellationToken_In_Context()
        {
            // Arrange
            
            var cancellationSource = new CancellationTokenSource();

            int called = 0;
            var mediator = Substitute.For<IMessageSendingMediator>();
            mediator.When(x => x.SendAsync(Arg.Any<MessageContext>()))
                .Do(x => {
                    called++;

                    Assert.Equal(cancellationSource.Token, x.Arg<MessageContext>().Cancellation);
                });

            var builder = GetBuilder<SimpleMessage>(sendingMediator: mediator);
            
            // Act
            builder.SetCancellationToken(cancellationSource.Token);
            await builder.SendAsync();

            // Assert
            Assert.Equal(1, called);
        }

        [Fact]
        public async Task Saves_Envelope_In_Context()
        {
            // Arrange

            var envelope = TestObjects.Envelope<SimpleMessage>();

            int called = 0;
            var mediator = Substitute.For<IMessageSendingMediator>();
            mediator.When(x => x.SendAsync(Arg.Any<MessageContext>()))
                .Do(x => {
                    called++;

                    Assert.Equal(envelope, x.Arg<MessageContext>().Envelope);
                });

            var builder = GetBuilder<SimpleMessage>(envelope: envelope, sendingMediator: mediator);

            // Act
            await builder.SendAsync();

            // Assert
            Assert.Equal(1, called);
        }
    }
}