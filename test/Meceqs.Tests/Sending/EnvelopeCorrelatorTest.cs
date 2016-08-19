using System;
using Meceqs.Sending;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class EnvelopeCorrelatorTest
    {
        private IEnvelopeCorrelator GetCorrelator()
        {
            return new DefaultEnvelopeCorrelator();
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

            var source = TestObjects.Envelope<SimpleMessage>();
            source.CorrelationId = Guid.NewGuid();

            var target = TestObjects.Envelope<SimpleMessage>();

            // Act
            correlator.CorrelateSourceWithTarget(source, target);

            // Assert
            target.CorrelationId.ShouldBe(source.CorrelationId);
        }
    }
}