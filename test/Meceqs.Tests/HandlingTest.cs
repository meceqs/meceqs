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

        public class MyCommandHandler : IHandles<MyCommand, VoidType>
        {
            public Task<VoidType> HandleAsync(HandleContext<MyCommand> ctx)
            {
                throw new NotImplementedException();
            }
        }

        private IEnvelopeHandler GetMediator(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            return new DefaultEnvelopeHandler(serviceProvider);
        }

        [Fact]
        public async Task FailingTest()
        {
            // Arrange

            var handler = Substitute.For<IHandles<MyCommand, VoidType>>();

            var services = new ServiceCollection().AddSingleton(handler);

            var mediator = GetMediator(services);

            var envelope = TestObjects.Envelope<MyCommand>();

            // Act
            await mediator.HandleAsync(envelope, CancellationToken.None);

            // Assert
            await handler.Received(1).HandleAsync(Arg.Any<HandleContext<MyCommand>>());
        }
    }
}