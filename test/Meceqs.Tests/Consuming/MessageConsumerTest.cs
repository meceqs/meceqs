using Meceqs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Meceqs.Tests.Consuming
{
    public class MessageConsumerTest
    {
        private IMessageConsumer GetMessageConsumer()
        {
            var services = new ServiceCollection()
                .AddOptions();

            var serviceProvider = services.BuildServiceProvider();

            return new MessageConsumer(serviceProvider);
        }

        [Fact]
        public void ForEnvelope_CreatesConsumeBuilder()
        {
            // Arrange
            var consumer = GetMessageConsumer();
            var envelope = TestObjects.Envelope<SimpleMessage>();

            // Act
            var builder = consumer.ForEnvelope(envelope);
        }
    }
}