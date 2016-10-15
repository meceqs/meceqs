using System;
using System.Threading.Tasks;
using Meceqs.Consuming;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Consuming
{
    public class MessageConsumerTest
    {
        private IMessageConsumer GetMessageConsumer(Action<FilterContext> callback = null)
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddOptions();

            services.AddMeceqs()
                .AddConsumePipeline(pipeline => pipeline.RunCallback(callback));

            var serviceProvider = services.BuildServiceProvider();

            return new MessageConsumer(serviceProvider);
        }

        private Envelope GetEmptyEnvelope<TMessage>() where TMessage : class, new()
        {
            var envelope = new Envelope<TMessage>();
            envelope.Message = new TMessage();
            return envelope;
        }

        private Task AssertFilterContext(Envelope envelope, Action<FilterContext> assertions)
        {
            return AssertFilterContext(envelope, null, assertions);
        }

        private async Task AssertFilterContext(Envelope envelope, Type resultType, Action<FilterContext> assertions)
        {
            // Assert callback
            var called = false;
            var callback = new Action<FilterContext>(ctx =>
            {
                called = true;
                assertions(ctx);
            });

            // Arrange
            var consumer = GetMessageConsumer(callback);

            // Act
            await (resultType == null ? consumer.ConsumeAsync(envelope) : consumer.ConsumeAsync(envelope, resultType));

            called.ShouldBeTrue();
        }

        [Fact]
        public void Missing_envelope_throws()
        {
            // Arrange
            var consumer = GetMessageConsumer();

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => consumer.ForEnvelope(null));
        }

        [Fact]
        public void Envelope_without_message_throws()
        {
            // Arrange
            var envelope = new Envelope<SimpleMessage>();
            var consumer = GetMessageConsumer();

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => consumer.ForEnvelope(envelope));
        }

        [Fact]
        public async Task Envelope_without_messageId_gets_new_messageId()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();

            await AssertFilterContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageId.ShouldNotBe(Guid.Empty);
            });
        }

        [Fact]
        public async Task Envelope_without_correlationId_reuses_messageId()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();
            envelope.MessageId = Guid.NewGuid();

            await AssertFilterContext(envelope, (ctx) =>
            {
                ctx.Envelope.CorrelationId.ShouldBe(envelope.MessageId);
            });
        }

        [Fact]
        public async Task Envelope_without_messageId_gets_new_messageId_and_equal_correlationId()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();

            await AssertFilterContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageId.ShouldNotBe(Guid.Empty);
                ctx.Envelope.MessageId.ShouldBe(ctx.Envelope.CorrelationId);
            });
        }

        [Fact]
        public async Task Envelope_without_messageType_gets_type_from_message()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();

            await AssertFilterContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageType.ShouldBe(typeof(SimpleMessage).FullName);
            });
        }

        [Fact]
        public async Task Envelope_with_wrong_messageType_gets_type_from_message()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();
            envelope.MessageType = "wrong-value";

            await AssertFilterContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageType.ShouldBe(typeof(SimpleMessage).FullName);
            });
        }

        [Fact]
        public async Task Envelope_without_createdOn_gets_current_utc_date()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();
            DateTime now = DateTime.UtcNow;

            await AssertFilterContext(envelope, (ctx) =>
            {
                ctx.Envelope.CreatedOnUtc.HasValue.ShouldBeTrue();

                var age = ctx.Envelope.CreatedOnUtc.Value - now;
                age.ShouldBeLessThan(TimeSpan.FromSeconds(5));
            });
        }

        [Fact]
        public async Task ResultType_is_passed_to_FilterContext()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var resultType = typeof(string);

            await AssertFilterContext(envelope, resultType, (ctx) =>
            {
                ctx.ExpectedResultType.ShouldBe(resultType);
            });
        }

        [Fact]
        public async Task Result_from_filter_is_returned()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();

            var callback = new Action<FilterContext>((ctx) => ctx.Result = "result");
            var consumer = GetMessageConsumer(callback);

            string result = await consumer.ConsumeAsync<string>(envelope);

            result.ShouldBe("result");
        }
    }
}