using System;
using Meceqs.Filters.TypedHandling;
using Meceqs.Filters.TypedHandling.Internal;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Filters.TypedHandling
{
    public class HandlerFactoryInvokerTest
    {
        private IHandlerFactoryInvoker GetInvoker()
        {
            return new DefaultHandlerFactoryInvoker();
        }

        [Fact]
        public void Throws_if_parameters_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var handlerFactory = Substitute.For<IHandlerFactory>();
            var resultType = typeof(string);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateHandler(null, typeof(SimpleMessage), resultType));
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateHandler(handlerFactory, null, resultType));
        }

        [Fact]
        public void Succeeds_for_Message_without_result()
        {
            // Arrange
            var invoker = GetInvoker();
            var handlerFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), null);

            // Assert
            handlerFactory.Received(1).CreateHandler<SimpleMessage>();
        }

        [Fact]
        public void Succeeds_for_Message_and_string()
        {
            // Arrange
            var invoker = GetInvoker();
            var handlerFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), typeof(string));

            // Assert
            handlerFactory.Received(1).CreateHandler<SimpleMessage, string>();
        }

        [Fact]
        public void Succeeds_for_Message_and_ComplexResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var handlerFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), typeof(SimpleResult));

            // Assert
            handlerFactory.Received(1).CreateHandler<SimpleMessage, SimpleResult>();
        }

        [Fact]
        public void Succeeds_if_called_multiple_times_with_same_types()
        {
            // Arrange
            var invoker = GetInvoker();
            var handlerFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), typeof(SimpleResult));
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), typeof(SimpleResult));
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), typeof(SimpleResult));

            // Assert
            handlerFactory.Received(3).CreateHandler<SimpleMessage, SimpleResult>();
        }

        [Fact]
        public void Succeeds_if_called_multiple_times_with_different_types()
        {
            // Arrange
            var invoker = GetInvoker();
            var handlerFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), typeof(string));
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleCommand), typeof(SimpleResult));
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleMessage), null);
            invoker.InvokeCreateHandler(handlerFactory, typeof(SimpleCommand), typeof(SimpleResult));

            // Assert
            handlerFactory.Received(1).CreateHandler<SimpleMessage, string>();
            handlerFactory.Received(1).CreateHandler<SimpleMessage>();
            handlerFactory.Received(2).CreateHandler<SimpleCommand, SimpleResult>();
        }
    }
}