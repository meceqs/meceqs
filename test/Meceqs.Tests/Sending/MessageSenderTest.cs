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
        private IMessageSender GetSender(IEnvelopeCorrelator envelopeCorrelator = null, ISendPipeline sendPipeline = null)
        {
            envelopeCorrelator = envelopeCorrelator ?? new DefaultEnvelopeCorrelator();
            sendPipeline = sendPipeline ?? Substitute.For<ISendPipeline>();

            return new MessageSender(TestObjects.EnvelopeFactory(), envelopeCorrelator, new DefaultFilterContextFactory(), sendPipeline);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            var envelopeFactory = Substitute.For<IEnvelopeFactory>();
            var correlator = Substitute.For<IEnvelopeCorrelator>();
            var filterContextFactory = Substitute.For<IFilterContextFactory>();
            var sendPipeline = Substitute.For<ISendPipeline>();

            Assert.Throws<ArgumentNullException>(() => new MessageSender(null, correlator, filterContextFactory, sendPipeline));
            Assert.Throws<ArgumentNullException>(() => new MessageSender(envelopeFactory, null, filterContextFactory, sendPipeline));
            Assert.Throws<ArgumentNullException>(() => new MessageSender(envelopeFactory, correlator, null, sendPipeline));
            Assert.Throws<ArgumentNullException>(() => new MessageSender(envelopeFactory, correlator, filterContextFactory, null));
        }

        [Fact]
        public async Task Calls_Pipeline()
        {
            // Arrange
            var sourceCmd = TestObjects.Envelope<SimpleCommand>();
            var resultEvent = new SimpleEvent();

            var sendPipeline = Substitute.For<ISendPipeline>();
            var sender = GetSender(sendPipeline: sendPipeline);

            // Act
            string result = await sender.ForEvent(resultEvent, Guid.NewGuid(), sourceCmd)
                .SendAsync<string>();

            // Assert
            await sendPipeline.Pipeline.ReceivedWithAnyArgs(1).ProcessAsync<string>(null);
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

            var sendPipeline = Substitute.For<ISendPipeline>();

            var sender = GetSender(sendPipeline: sendPipeline);
            var cancellationSource = new CancellationTokenSource();

            IList<FilterContext> filterContexts = null;
            await sendPipeline.Pipeline.ProcessAsync<string>(Arg.Do<IList<FilterContext>>(x => filterContexts = x));

            // Act

            string result = await sender.ForEvent(resultEvent, resultEventId, sourceCmd)
                .SetCancellationToken(cancellationSource.Token)
                .SetHeader("Key", "Value")
                .SetContextItem("SendKey", "SendValue")
                .SendAsync<string>();

            // Assert

            await sendPipeline.Pipeline.ReceivedWithAnyArgs(1).ProcessAsync<string>(null);

            Assert.NotNull(filterContexts);
            Assert.Equal(1, filterContexts.Count);
            
            var context = filterContexts[0];
            Assert.Equal(resultEventId, context.Envelope.MessageId);
            Assert.Equal(resultEvent, context.Envelope.Message);
            Assert.Equal(cancellationSource.Token, context.Cancellation);
            Assert.Equal("Value", context.Envelope.Headers["Key"]);
            Assert.Equal("SendValue", context.GetContextItem<string>("SendKey"));
        }
    }
}