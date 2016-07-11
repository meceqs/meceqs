using Meceqs.AzureServiceBus;
using Microsoft.ServiceBus.Messaging;
using Xunit;

namespace Meceqs.Tests.AzureServiceBus
{
    public class BrokeredMessageConverterTest
    {
        [Fact]
        public void bla()
        {
            // Arrange
            var sentEnvelope = TestObjects.Envelope(new SimpleMessage { SomeKey = "test" });
            BrokeredMessage brokeredMessage = new BrokeredMessage(sentEnvelope);

            // Act
            var converter = new DefaultBrokeredMessageConverter(null, null);
            var receivedEnvelope = converter.ConvertToEnvelope(brokeredMessage);

            // Assert

            Assert.IsType(typeof(Envelope<SimpleMessage>), receivedEnvelope);

            var typedEnvelope = (Envelope<SimpleMessage>) receivedEnvelope;
            Assert.Same(sentEnvelope, receivedEnvelope);
            Assert.Equal("test", typedEnvelope.Message.SomeKey);

        }
    }
}