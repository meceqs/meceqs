using System;
using System.Threading;
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
        public void Create_throws_if_parameters_missing()
        {
            // Arrange
            var factory = GetFactory();
            var envelope = TestObjects.Envelope<SimpleMessage>();
            var messageContextData = new MessageContextData();
            var cancellation = CancellationToken.None;

            Assert.Throws<ArgumentNullException>(() => factory.Create(null, messageContextData, cancellation));
            Assert.Throws<ArgumentNullException>(() => factory.Create(envelope, null, cancellation));
        }
    }
}