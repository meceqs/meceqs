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
            var serviceProvider = Substitute.For<IServiceProvider>();

            serviceProvider.GetService(typeof(IEnvelopeFactory)).Returns(new DefaultEnvelopeFactory());

            serviceProvider.GetService(typeof(IEnvelopeCorrelator)).Returns(envelopeCorrelator ?? Substitute.For<IEnvelopeCorrelator>());

            var pipelineProvider = Substitute.For<IPipelineProvider>();
            pipelineProvider.GetPipeline(Arg.Any<string>()).Returns(pipeline ?? Substitute.For<IPipeline>());
            serviceProvider.GetService(typeof(IPipelineProvider)).Returns(pipelineProvider);

            return new MessageSender(serviceProvider);
        }

        [Fact]
        public void Throws_if_parameters_are_missing()
        {
            Should.Throw<ArgumentNullException>(() => new MessageSender(null));
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
            await pipeline.ReceivedWithAnyArgs(1).InvokeAsync(null);
        }

        [Fact]
        public async Task Saves_settings_in_MessageContext()
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
            pipeline.WhenForAnyArgs(x => x.InvokeAsync(null))
                .Do(x =>
                {
                    called++;

                    var ctx = x.Arg<MessageContext>();
                    Assert.Equal(resultEventId, ctx.Envelope.MessageId);
                    Assert.Equal(resultEvent, ctx.Envelope.Message);
                    Assert.Equal(cancellationSource.Token, ctx.Cancellation);
                    Assert.Equal("Value", ctx.Envelope.Headers["Key"]);
                    Assert.Equal("SendValue", ctx.Items.Get<string>("SendKey"));
                });

            var sender = GetSender(pipeline: pipeline);

            MessageContext messageContext = null;
            await pipeline.InvokeAsync(Arg.Do<MessageContext>(x => messageContext = x));

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