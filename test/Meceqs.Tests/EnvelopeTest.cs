using System;
using Shouldly;
using Xunit;

namespace Meceqs.Tests
{
    public class EnvelopeTest
    {
        private Envelope<T> GetEnvelope<T>() where T : class, new()
        {
            return new Envelope<T>(new T(), Guid.NewGuid());
        }

        [Fact]
        public void Ctor_throws_if_message_missing()
        {
            Should.Throw<ArgumentNullException>(() => new Envelope<SimpleMessage>(null, Guid.NewGuid()));
        }

        [Fact]
        public void Ctor_throws_if_messageId_missing()
        {
            Should.Throw<ArgumentNullException>(() => new Envelope<SimpleMessage>(new SimpleMessage(), Guid.Empty));
        }

        [Fact]
        public void Ctor_sets_properties()
        {
            var message = new SimpleMessage();
            var messageId = Guid.NewGuid();
            var env = new Envelope<SimpleMessage>(message, messageId);

            env.Message.ShouldBe(message);
            env.MessageId.ShouldBe(messageId);
            env.MessageName.ShouldBe("SimpleMessage");
            env.MessageType.ShouldBe("Meceqs.Tests.SimpleMessage");
            env.CorrelationId.ShouldNotBeNull();
            env.CreatedOnUtc.ShouldNotBeNull();
        }

        [Fact]
        public void EnsureValid_throws_if_message_missing()
        {
            var env = GetEnvelope<SimpleMessage>();
            env.Message = null;

            Should.Throw<ArgumentNullException>(() => env.EnsureValid());
        }

        [Fact]
        public void EnsureValid_throws_if_messageId_missing()
        {
            var env = GetEnvelope<SimpleMessage>();
            env.MessageId = Guid.Empty;

            Should.Throw<ArgumentNullException>(() => env.EnsureValid());
        }

        [Fact]
        public void EnsureValid_throws_if_messageType_missing()
        {
            var env = GetEnvelope<SimpleMessage>();
            env.MessageType = null;

            Should.Throw<ArgumentNullException>(() => env.EnsureValid());
        }

        [Fact]
        public void EnsureValid_throws_if_messageName_missing()
        {
            var env = GetEnvelope<SimpleMessage>();
            env.MessageName = null;

            Should.Throw<ArgumentNullException>(() => env.EnsureValid());
        }
    }
}