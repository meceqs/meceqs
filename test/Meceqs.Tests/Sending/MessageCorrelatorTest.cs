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

        [Fact]
        public void Doesnt_throw_if_parameter_is_null()
        {
            // Arrange
            var correlator = GetCorrelator();
            var envelope = TestObjects.Envelope<SimpleMessage>();

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

            var source = TestObjects.Envelope<SimpleMessage>();
            source.CorrelationId = correlationId;

            var target = TestObjects.Envelope<SimpleMessage>();

            // Act
            correlator.CorrelateSourceWithTarget(source, target);

            // Assert
            Assert.Equal(correlationId, target.CorrelationId);
        }
    }
}