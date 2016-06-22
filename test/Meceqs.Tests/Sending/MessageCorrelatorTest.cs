using System;
using Meceqs.Sending;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class MessageCorrelatorTest
    {
        private IMessageCorrelator GetCorrelator()
        {
            return new DefaultMessageCorrelator();
        }

        private MessageEnvelope<TMessage> GetEnvelope<TMessage>(TMessage message = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return new MessageEnvelope<TMessage>(message, Guid.NewGuid());
        }

        [Fact]
        public void Doesnt_throw_if_parameter_is_null()
        {
            // Arrange
            var correlator = GetCorrelator();

            // Act & Assert
            correlator.CorrelateSourceWithTarget(null, Substitute.For<IMessageEnvelope>());
            correlator.CorrelateSourceWithTarget(Substitute.For<IMessageEnvelope>(), null);
        }

        [Fact]
        public void Sets_CorrelationId()
        {
            // Arrange

            var correlator = GetCorrelator();
            var correlationId = Guid.NewGuid();

            var source = GetEnvelope<SimpleMessage>();
            source.CorrelationId = correlationId;

            var target = GetEnvelope<SimpleMessage>();

            // Act
            correlator.CorrelateSourceWithTarget(source, target);

            // Assert
            Assert.Equal(correlationId, target.CorrelationId);
        }
    }
}