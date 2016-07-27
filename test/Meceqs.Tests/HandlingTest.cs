using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Handling;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests
{
    public class HandlingTest
    {
        public class MyCommand : ICommand
        {
            public string Property { get; set; }
        }

        public class MyCommandHandler : IHandler<MyCommand, VoidType>
        {
            public Task<VoidType> HandleAsync(MessageContext<MyCommand> ctx)
            {
                throw new NotImplementedException();
            }
        }

        private IEnvelopeHandler GetMediator(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            
            var handlerFactory = new DefaultHandlerFactory(serviceProvider);
            return new DefaultEnvelopeHandler(handlerFactory, new DefaultHandlerInvoker());
        }

        [Fact]
        public async Task FailingTest()
        {
            // Arrange

            var handler = Substitute.For<IHandler<MyCommand, VoidType>>();

            var services = new ServiceCollection().AddSingleton(handler);

            var mediator = GetMediator(services);

            var envelope = TestObjects.Envelope<MyCommand>();

            // Act
            await mediator.HandleAsync(envelope, CancellationToken.None);

            // Assert
            await handler.Received(1).HandleAsync(Arg.Any<MessageContext<MyCommand>>());
        }
    }
}