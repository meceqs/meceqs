using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Sending;
using Meceqs.Sending.Internal;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class FluentSenderTest
    {
        private IFluentSender GetFluentSender<TMessage>(
            Envelope<TMessage> envelope = null,
            IEnvelopeCorrelator correlator = null,
            IPipeline pipeline = null) where TMessage : class, new()
        {
            envelope = envelope ?? TestObjects.Envelope<TMessage>();
            correlator = correlator ?? new DefaultEnvelopeCorrelator();
            pipeline = pipeline ?? Substitute.For<IPipeline>();
            
            var pipelineProvider = Substitute.For<IPipelineProvider>();
            pipelineProvider.GetPipeline(Arg.Any<string>()).Returns(pipeline);

            var envelopes = new List<Envelope> { envelope };

            return new FluentSender(envelopes, correlator, new DefaultFilterContextFactory(), pipelineProvider);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            // Arrange
            var envelopes = new List<Envelope>();
            var correlator = new DefaultEnvelopeCorrelator();
            var filterContextFactory = new DefaultFilterContextFactory();
            var pipelineProvider = Substitute.For<IPipelineProvider>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FluentSender(null, correlator, filterContextFactory, pipelineProvider));
            Assert.Throws<ArgumentNullException>(() => new FluentSender(envelopes, null, filterContextFactory, pipelineProvider));
            Assert.Throws<ArgumentNullException>(() => new FluentSender(envelopes, correlator, null, pipelineProvider));
            Assert.Throws<ArgumentNullException>(() => new FluentSender(envelopes, correlator, filterContextFactory, null));
        }

        [Fact]
        public async Task Calls_Pipeline()
        {
            // Arrange
            var pipeline = Substitute.For<IPipeline>();

            var sender = GetFluentSender<SimpleMessage>(pipeline: pipeline);

            // Act
            await sender.SendAsync();

            // Assert
            await pipeline.Received(1).ProcessAsync(Arg.Any<IList<FilterContext>>());
        }

        [Fact]
        public void Calls_Correlator()
        {
            // Arrange
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var correlator = Substitute.For<IEnvelopeCorrelator>();
            var sender = GetFluentSender<SimpleMessage>(envelope, correlator);

            // Act
            var sourceMsg = TestObjects.Envelope<SimpleCommand>();
            sender.CorrelateWith(sourceMsg);

            // Assert
            correlator.Received(1).CorrelateSourceWithTarget(sourceMsg, envelope);
        }

        [Fact]
        public async Task Saves_Header_In_Envelope()
        {
            // Arrange

            int called = 0;
            var pipeline = Substitute.For<IPipeline>();
            pipeline.When(x => x.ProcessAsync(Arg.Any<IList<FilterContext>>()))
                .Do(x => {
                    called++;

                    var ctx = x.Arg<IList<FilterContext>>().First();
                    Assert.Equal("Value", ctx.Envelope.Headers["Key"]);
                });

            var builder = GetFluentSender<SimpleMessage>(pipeline: pipeline);

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
            var pipeline = Substitute.For<IPipeline>();
            pipeline.When(x => x.ProcessAsync(Arg.Any<IList<FilterContext>>()))
                .Do(x => {
                    called++;
                    
                    var ctx = x.Arg<IList<FilterContext>>().First();
                    Assert.Equal("Value", ctx.Items.Get<string>("Key"));
                });

            var sender = GetFluentSender<SimpleMessage>(pipeline: pipeline);

            // Act
            sender.SetContextItem("Key", "Value");
            await sender.SendAsync();

            // Assert
            Assert.Equal(1, called);
        }

        [Fact]
        public async Task Saves_CancellationToken_In_Context()
        {
            // Arrange
            
            var cancellationSource = new CancellationTokenSource();

            int called = 0;
            var pipeline = Substitute.For<IPipeline>();
            pipeline.When(x => x.ProcessAsync(Arg.Any<IList<FilterContext>>()))
                .Do(x => {
                    called++;

                    var ctx = x.Arg<IList<FilterContext>>().First();
                    Assert.Equal(cancellationSource.Token, ctx.Cancellation);
                });

            var builder = GetFluentSender<SimpleMessage>(pipeline: pipeline);
            
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
            var pipeline = Substitute.For<IPipeline>();
            pipeline.When(x => x.ProcessAsync(Arg.Any<IList<FilterContext>>()))
                .Do(x => {
                    called++;

                    var ctx = x.Arg<IList<FilterContext>>().First();
                    Assert.Equal(envelope, ctx.Envelope);
                });

            var sender = GetFluentSender<SimpleMessage>(envelope: envelope, pipeline: pipeline);

            // Act
            await sender.SendAsync();

            // Assert
            Assert.Equal(1, called);
        }
    }
}