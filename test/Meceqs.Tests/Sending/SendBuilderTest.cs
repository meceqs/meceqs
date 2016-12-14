using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Sending;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class SendBuilderTest
    {
        private IServiceProvider GetMessageContextBuilderServiceProvider(
            IEnvelopeCorrelator correlator = null,
            IPipeline pipeline = null
        )
        {
            var serviceProvider = Substitute.For<IServiceProvider>();

            serviceProvider.GetService(typeof(IEnvelopeFactory)).Returns(new DefaultEnvelopeFactory());
            serviceProvider.GetService(typeof(IMessageContextFactory)).Returns(new DefaultMessageContextFactory());

            serviceProvider.GetService(typeof(IEnvelopeCorrelator)).Returns(correlator ?? Substitute.For<IEnvelopeCorrelator>());

            var pipelineProvider = Substitute.For<IPipelineProvider>();
            pipelineProvider.GetPipeline(Arg.Any<string>()).Returns(pipeline ?? Substitute.For<IPipeline>());
            serviceProvider.GetService(typeof(IPipelineProvider)).Returns(pipelineProvider);

            return serviceProvider;
        }

        private ISendBuilder GetSendBuilder<TMessage>(
            Envelope<TMessage> envelope = null,
            IEnvelopeCorrelator correlator = null,
            IPipeline pipeline = null) where TMessage : class, new()
        {
            envelope = envelope ?? TestObjects.Envelope<TMessage>();

            var serviceProvider = GetMessageContextBuilderServiceProvider(correlator, pipeline);

            return new SendBuilder(envelope, serviceProvider);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            // Arrange
            var serviceProvider = GetMessageContextBuilderServiceProvider();
            serviceProvider.GetService(typeof(IEnvelopeCorrelator)).Returns(Substitute.For<IEnvelopeCorrelator>());

            // Act & Assert
            var envelope = TestObjects.Envelope<SimpleMessage>();
            Should.Throw<ArgumentNullException>(() => new SendBuilder((Envelope)null, serviceProvider));
            Should.Throw<ArgumentNullException>(() => new SendBuilder(envelope, null));
        }

        [Fact]
        public async Task Calls_Pipeline()
        {
            // Arrange
            var pipeline = Substitute.For<IPipeline>();

            var builder = GetSendBuilder<SimpleMessage>(pipeline: pipeline);

            // Act
            await builder.SendAsync();

            // Assert
            await pipeline.ReceivedWithAnyArgs(1).InvokeAsync(null);
        }

        [Fact]
        public void Calls_Correlator()
        {
            // Arrange
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var correlator = Substitute.For<IEnvelopeCorrelator>();
            var builder = GetSendBuilder<SimpleMessage>(envelope, correlator);

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
            var pipeline = Substitute.For<IPipeline>();
            pipeline.WhenForAnyArgs(x => x.InvokeAsync(null))
                .Do(x => {
                    called++;

                    var ctx = x.Arg<MessageContext>();
                    ctx.Envelope.Headers["Key"].ShouldBe("Value");
                });

            var builder = GetSendBuilder<SimpleMessage>(pipeline: pipeline);

            // Act
            builder.SetHeader("Key", "Value");
            await builder.SendAsync();

            // Assert
            called.ShouldBe(1);
        }

        [Fact]
        public async Task Saves_SendProperty_In_Context()
        {
            // Arrange
            int called = 0;
            var pipeline = Substitute.For<IPipeline>();
            pipeline.WhenForAnyArgs(x => x.InvokeAsync(null))
                .Do(x => {
                    called++;

                    var ctx = x.Arg<MessageContext>();
                    ctx.Items.Get<string>("Key").ShouldBe("Value");
                });

            var builder = GetSendBuilder<SimpleMessage>(pipeline: pipeline);

            // Act
            builder.SetContextItem("Key", "Value");
            await builder.SendAsync();

            // Assert
            called.ShouldBe(1);
        }

        [Fact]
        public async Task Saves_CancellationToken_In_Context()
        {
            // Arrange

            var cancellationSource = new CancellationTokenSource();

            int called = 0;
            var pipeline = Substitute.For<IPipeline>();
            pipeline.WhenForAnyArgs(x => x.InvokeAsync(null))
                .Do(x => {
                    called++;

                    var ctx = x.Arg<MessageContext>();
                    ctx.Cancellation.ShouldBe(cancellationSource.Token);
                });

            var builder = GetSendBuilder<SimpleMessage>(pipeline: pipeline);

            // Act
            builder.SetCancellationToken(cancellationSource.Token);
            await builder.SendAsync();

            // Assert
            called.ShouldBe(1);
        }

        [Fact]
        public async Task Saves_Envelope_In_Context()
        {
            // Arrange

            var envelope = TestObjects.Envelope<SimpleMessage>();

            int called = 0;
            var pipeline = Substitute.For<IPipeline>();
            pipeline.WhenForAnyArgs(x => x.InvokeAsync(null))
                .Do(x => {
                    called++;

                    var ctx = x.Arg<MessageContext>();
                    ctx.Envelope.ShouldBe(envelope);
                });

            var builder = GetSendBuilder<SimpleMessage>(envelope: envelope, pipeline: pipeline);

            // Act
            await builder.SendAsync();

            // Assert
            called.ShouldBe(1);
        }
    }
}