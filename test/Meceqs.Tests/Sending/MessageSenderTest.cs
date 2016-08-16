using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Sending;
using Meceqs.Sending.Internal;
using NSubstitute;
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

            Assert.Throws<ArgumentNullException>(() => new MessageSender(null, correlator, filterContextFactory, pipelineProvider));
            Assert.Throws<ArgumentNullException>(() => new MessageSender(envelopeFactory, null, filterContextFactory, pipelineProvider));
            Assert.Throws<ArgumentNullException>(() => new MessageSender(envelopeFactory, correlator, null, pipelineProvider));
            Assert.Throws<ArgumentNullException>(() => new MessageSender(envelopeFactory, correlator, filterContextFactory, null));
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
            await pipeline.Received(1).ProcessAsync<string>(Arg.Any<IList<FilterContext>>());
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

            var pipeline = Substitute.For<IPipeline>();

            var sender = GetSender(pipeline: pipeline);
            var cancellationSource = new CancellationTokenSource();

            IList<FilterContext> filterContexts = null;
            await pipeline.ProcessAsync<string>(Arg.Do<IList<FilterContext>>(x => filterContexts = x));

            // Act

            string result = await sender.ForMessage(resultEvent, resultEventId)
                .CorrelateWith(sourceCmd)
                .SetCancellationToken(cancellationSource.Token)
                .SetHeader("Key", "Value")
                .SetContextItem("SendKey", "SendValue")
                .SendAsync<string>();

            // Assert

            await pipeline.Received(1).ProcessAsync<string>(Arg.Any<IList<FilterContext>>());

            Assert.NotNull(filterContexts);
            Assert.Equal(1, filterContexts.Count);
            
            var context = filterContexts[0];
            Assert.Equal(resultEventId, context.Envelope.MessageId);
            Assert.Equal(resultEvent, context.Envelope.Message);
            Assert.Equal(cancellationSource.Token, context.Cancellation);
            Assert.Equal("Value", context.Envelope.Headers["Key"]);
            Assert.Equal("SendValue", context.Items.Get<string>("SendKey"));
        }
    }
}