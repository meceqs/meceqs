using System.Threading;
using System.Threading.Tasks;
using Meceqs.Sending.Transport;
using Meceqs.Sending.Transport.TypedSend;
using Meceqs.ServiceProviderIntegration;
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
            var serviceProviderFactory = new ServiceProviderFactory(serviceProvider);

            return new TypedSendTransport(serviceProviderFactory);
        }

        private MessageContext<TMessage> GetSendContext<TMessage>()
            where TMessage : class, IMessage, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();
            return new MessageContext<TMessage>(envelope, new MessageContextData(), CancellationToken.None);
        }

        [Fact]
        public async Task Calls_Matching_Handler()
        {
            // Arrange
            var handler = Substitute.For<ISender<SimpleMessage, string>>();
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