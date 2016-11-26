using System;
using Meceqs.Pipeline;
using Shouldly;
using Xunit;

namespace Meceqs.Tests
{
    public class MessageContextFactoryTest
    {
        private IMessageContextFactory GetFactory()
        {
            return new DefaultMessageContextFactory();
        }

        [Fact]
        public void Throws_if_parameters_missing()
        {
            var factory = GetFactory();

            Should.Throw<ArgumentNullException>(() => factory.CreateMessageContext(null));
        }

        [Fact]
        public void Creates_MessageContext_with_same_generic_MessageType()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var factory = GetFactory();

            var messageContext = factory.CreateMessageContext(envelope);

            messageContext.ShouldBeOfType<MessageContext<SimpleMessage>>();
        }

        [Fact]
        public void Creates_MessageContext_with_same_envelope()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var factory = GetFactory();

            var messageContext = factory.CreateMessageContext(envelope);

            messageContext.Envelope.ShouldBe(envelope);
            messageContext.Message.ShouldBe(envelope.Message);
        }

        [Fact]
        public void Succeeds_multiple_times_for_same_envelope()
        {
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var factory = GetFactory();

            factory.CreateMessageContext(envelope);
            factory.CreateMessageContext(envelope);
            factory.CreateMessageContext(envelope);
        }

        [Fact]
        public void Succeeds_multiple_times_for_different_envelopes()
        {
            var envelope1 = TestObjects.Envelope<SimpleMessage>();
            var envelope2 = TestObjects.Envelope<SimpleCommand>();
            var factory = GetFactory();

            factory.CreateMessageContext(envelope1);
            factory.CreateMessageContext(envelope2);
            factory.CreateMessageContext(envelope1);
            factory.CreateMessageContext(envelope2);
        }
    }
}