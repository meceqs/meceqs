using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Meceqs.Receiving;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Receiving
{
    public class MessageReceiverTest
    {
        private IMessageReceiver GetMessageReceiver(Action<MessageContext> callback = null)
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddOptions();

            services.AddMeceqs(builder =>
            {
                builder.AddReceivePipeline(pipeline => pipeline.RunCallback(callback));
            });

            var serviceProvider = services.BuildServiceProvider();

            return new MessageReceiver(serviceProvider);
        }

        private Envelope GetEmptyEnvelope<TMessage>() where TMessage : class, new()
        {
            var envelope = new Envelope<TMessage>();
            envelope.Message = new TMessage();
            return envelope;
        }

        private Task AssertMessageContext(Envelope envelope, Action<MessageContext> assertions)
        {
            return AssertMessageContext(envelope, null, assertions);
        }

        private async Task AssertMessageContext(Envelope envelope, Type responseType, Action<MessageContext> assertions)
        {
            // Assert callback
            var called = false;
            var callback = new Action<MessageContext>(ctx =>
            {
                called = true;
                assertions(ctx);
            });

            // Arrange
            var receiver = GetMessageReceiver(callback);

            // Act
            await (responseType == null ? receiver.ReceiveAsync(envelope) : receiver.ReceiveAsync(envelope, responseType));

            called.ShouldBeTrue();
        }

        [Fact]
        public void Missing_envelope_throws()
        {
            // Arrange
            var receiver = GetMessageReceiver();

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => receiver.ForEnvelope(null));
        }

        [Fact]
        public void Envelope_without_message_throws()
        {
            // Arrange
            var envelope = new Envelope<SimpleMessage>();
            var receiver = GetMessageReceiver();

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => receiver.ForEnvelope(envelope));
        }

        [Fact]
        public async Task Envelope_without_messageId_gets_new_messageId()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();

            await AssertMessageContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageId.ShouldNotBe(Guid.Empty);
            });
        }

        [Fact]
        public async Task Envelope_without_correlationId_reuses_messageId()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();
            envelope.MessageId = Guid.NewGuid();

            await AssertMessageContext(envelope, (ctx) =>
            {
                ctx.Envelope.CorrelationId.ShouldBe(envelope.MessageId);
            });
        }

        [Fact]
        public async Task Envelope_without_messageId_gets_new_messageId_and_equal_correlationId()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();

            await AssertMessageContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageId.ShouldNotBe(Guid.Empty);
                ctx.Envelope.MessageId.ShouldBe(ctx.Envelope.CorrelationId);
            });
        }

        [Fact]
        public async Task Envelope_without_messageType_gets_type_from_message()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();

            await AssertMessageContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageType.ShouldBe(typeof(SimpleMessage).FullName);
            });
        }

        [Fact]
        public async Task Envelope_with_wrong_messageType_gets_type_from_message()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();
            envelope.MessageType = "wrong-value";

            await AssertMessageContext(envelope, (ctx) =>
            {
                ctx.Envelope.MessageType.ShouldBe(typeof(SimpleMessage).FullName);
            });
        }

        [Fact]
        public async Task Envelope_without_createdOn_gets_current_utc_date()
        {
            var envelope = GetEmptyEnvelope<SimpleMessage>();
            DateTime now = DateTime.UtcNow;

            await AssertMessageContext(envelope, (ctx) =>
            {
                ctx.Envelope.CreatedOnUtc.HasValue.ShouldBeTrue();

                var age = ctx.Envelope.CreatedOnUtc.Value - now;
                age.ShouldBeLessThan(TimeSpan.FromSeconds(5));
            });
        }

        [Fact]
        public async Task ResponseType_is_passed_to_MessageContext()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var responseType = typeof(string);

            await AssertMessageContext(envelope, responseType, (ctx) =>
            {
                ctx.ExpectedResponseType.ShouldBe(responseType);
            });
        }

        [Fact]
        public async Task Response_from_middleware_is_returned()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();

            var callback = new Action<MessageContext>((ctx) => ctx.Response = "response");
            var receiver = GetMessageReceiver(callback);

            string response = await receiver.ReceiveAsync<string>(envelope);

            response.ShouldBe("response");
        }
    }
}
