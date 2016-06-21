using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Consuming;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests
{
    public class Test1
    {
        public class MyCommand : ICommand
        {
            public string Property { get; set; }
        }

        public class MyCommandHandler : IConsumes<MyCommand, VoidType>
        {
            public Task<VoidType> ConsumeAsync(ConsumeContext<MyCommand> ctx)
            {
                throw new NotImplementedException();
            }
        }

        private IMessageConsumer GetMediator(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            return new DefaultMessageConsumer(serviceProvider);
        }

        private MessageEnvelope<TMessage> GetEnvelope<TMessage>() where TMessage : IMessage, new()
        {
            var msg = new TMessage();
            return new MessageEnvelope<TMessage>(Guid.NewGuid(), msg);
        }

        [Fact]
        public async void FailingTest()
        {
            // Arrange

            var handler = Substitute.For<IConsumes<MyCommand, VoidType>>();

            var services = new ServiceCollection().AddSingleton(handler);

            var mediator = GetMediator(services);

            var envelope = GetEnvelope<MyCommand>();

            // Act
            await mediator.ConsumeAsync(envelope, CancellationToken.None);

            // Assert
            await handler.Received(1).ConsumeAsync(Arg.Any<ConsumeContext<MyCommand>>());
        }
    }
}