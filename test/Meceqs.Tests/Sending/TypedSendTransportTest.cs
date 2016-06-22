using System;
using System.Threading.Tasks;
using Meceqs.Sending;
using Meceqs.Sending.TypedSend;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending
{
    public class TypedSendTransportTest
    {
        private ISendTransport GetTransport(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            return new TypedSendTransport(serviceProvider);
        }

        private SendContext<TMessage> GetSendContext<TMessage>()
            where TMessage : class, IMessage, new()
        {
            var envelope = new MessageEnvelope<TMessage>(new TMessage(), Guid.NewGuid());
            return new SendContext<TMessage>(envelope, new SendProperties(), null);
        }

        [Fact]
        public async Task Calls_Matching_Handler()
        {
            // Arrange
            var handler = Substitute.For<ISends<SimpleMessage, string>>();
            var services = new ServiceCollection().AddSingleton(handler);
            var transport = GetTransport(services);

            var sendContext = GetSendContext<SimpleMessage>();

            // Act
            string result = await transport.SendAsync<SimpleMessage, string>(sendContext);

            // Assert
            await handler.Received(1).SendAsync(sendContext);
        }
    }
}