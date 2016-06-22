using System;
using Meceqs.Sending;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class MessageCorrelatorTest
    {
        private IMessageCorrelator GetCorrelator()
        {
            return new DefaultMessageCorrelator();
        }

        private Envelope<TMessage> GetEnvelope<TMessage>(TMessage message = null)
            where TMessage : class, IMessage, new()
        {
            message = message ?? new TMessage();

            return new DefaultEnvelopeFactory().Create(message, Guid.NewGuid());
        }

        [Fact]
        public void Doesnt_throw_if_parameter_is_null()
        {
            // Arrange
            var correlator = GetCorrelator();
            var envelope = GetEnvelope<SimpleMessage>();

            // Act & Assert
            correlator.CorrelateSourceWithTarget(null, envelope);
            correlator.CorrelateSourceWithTarget(envelope, null);
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