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
    public class MessageSenderTest
    {
        private IMessageSender GetSender(IEnvelopeCorrelator envelopeCorrelator = null, IPipeline pipeline = null)
        {
            envelopeCorrelator = envelopeCorrelator ?? new DefaultEnvelopeCorrelator();
            pipeline = pipeline ?? Substitute.For<IPipeline>();

            var pipelineProvider = Substitute.For<IPipelineProvider>();
            pipelineProvider.GetPipeline(Arg.Any<string>()).Returns(pipeline);

            return new MessageSender(new DefaultEnvelopeFactory(), envelopeCorrelator, new DefaultFilterContextFactory(), pipelineProvider);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            var envelopeFactory = Substitute.For<IEnvelopeFactory>();
            var correlator = Substitute.For<IEnvelopeCorrelator>();
            var filterContextFactory = Substitute.For<IFilterContextFactory>();
            var pipelineProvider = Substitute.For<IPipelineProvider>();

            Should.Throw<ArgumentNullException>(() => new MessageSender(null, correlator, filterContextFactory, pipelineProvider));
            Should.Throw<ArgumentNullException>(() => new MessageSender(envelopeFactory, null, filterContextFactory, pipelineProvider));
            Should.Throw<ArgumentNullException>(() => new MessageSender(envelopeFactory, correlator, null, pipelineProvider));
            Should.Throw<ArgumentNullException>(() => new MessageSender(envelopeFactory, correlator, filterContextFactory, null));
        }

        [Fact]
        public async Task Calls_Pipeline()
        {
            // Arrange
            var resultEvent = new SimpleEvent();

            var pipeline = Substitute.For<IPipeline>();
            var sender = GetSender(pipeline: pipeline);

            // Act
            string result = await sender.ForMessage(resultEvent)
                .SendAsync<string>();

            // Assert
            await pipeline.ReceivedWithAnyArgs(1).ProcessAsync<string>(null);
        }

        [Fact]
        public async Task Saves_settings_in_FilterContext()
        {
            // Arrange

            var correlationId = Guid.NewGuid();
            var sourceCmd = TestObjects.Envelope<SimpleCommand>();
            sourceCmd.CorrelationId = correlationId;

            var resultEvent = new SimpleEvent();
            var resultEventId = Guid.NewGuid();

            var cancellationSource = new CancellationTokenSource();

            var called = 0;
            var pipeline = Substitute.For<IPipeline>();
            pipeline.WhenForAnyArgs(x => x.ProcessAsync<string>(null))
                .Do(x =>
                {
                    called++;

                    var ctx = x.Arg<FilterContext>();
                    Assert.Equal(resultEventId, ctx.Envelope.MessageId);
                    Assert.Equal(resultEvent, ctx.Envelope.Message);
                    Assert.Equal(cancellationSource.Token, ctx.Cancellation);
                    Assert.Equal("Value", ctx.Envelope.Headers["Key"]);
                    Assert.Equal("SendValue", ctx.Items.Get<string>("SendKey"));
                });

            var sender = GetSender(pipeline: pipeline);

            FilterContext filterContext = null;
            await pipeline.ProcessAsync(Arg.Do<FilterContext>(x => filterContext = x));

            // Act

            string result = await sender.ForMessage(resultEvent, resultEventId)
                .CorrelateWith(sourceCmd)
                .SetCancellationToken(cancellationSource.Token)
                .SetHeader("Key", "Value")
                .SetContextItem("SendKey", "SendValue")
                .SendAsync<string>();

            // Assert
            Assert.Equal(1, called);
        }
    }
}