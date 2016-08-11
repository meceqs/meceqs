using System;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending.TypedSend
{
    public class SenderFactoryInvokerTest
    {
        private IHandlerFactoryInvoker GetInvoker()
        {
            return new DefaultHandlerFactoryInvoker();
        }

        [Fact]
        public void CreateSender_throws_if_parameters_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<IHandlerFactory>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateHandler(null, typeof(SimpleMessage), typeof(VoidType)));
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateHandler(senderFactory, null, typeof(VoidType)));
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), null));
        }

        [Fact]
        public void CreateSender_succeeds_for_Message_and_VoidType()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(VoidType));

            // Assert
            senderFactory.Received(1).CreateHandler<SimpleMessage, VoidType>();
        }

        [Fact]
        public void CreateSender_succeeds_for_Message_and_string()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(string));

            // Assert
            senderFactory.Received(1).CreateHandler<SimpleMessage, string>();
        }

        [Fact]
        public void CreateSender_succeeds_for_Message_and_ComplexResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));

            // Assert
            senderFactory.Received(1).CreateHandler<SimpleMessage, SimpleResult>();
        }

        [Fact]
        public void CreateSender_succeeds_if_called_multiple_times_with_same_types()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));

            // Assert
            senderFactory.Received(3).CreateHandler<SimpleMessage, SimpleResult>();
        }

        [Fact]
        public void CreateSender_succeeds_if_called_multiple_times_with_different_types()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<IHandlerFactory>();

            // Act
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(string));
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleCommand), typeof(SimpleResult));
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleMessage), typeof(VoidType));
            invoker.InvokeCreateHandler(senderFactory, typeof(SimpleCommand), typeof(SimpleResult));

            // Assert
            senderFactory.Received(1).CreateHandler<SimpleMessage, string>();
            senderFactory.Received(1).CreateHandler<SimpleMessage, VoidType>();
            senderFactory.Received(2).CreateHandler<SimpleCommand, SimpleResult>();
        }
    }
}