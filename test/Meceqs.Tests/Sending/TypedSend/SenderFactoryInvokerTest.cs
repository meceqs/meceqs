using System;
using Meceqs.Sending.TypedSend;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending.TypedSend
{
    public class SenderFactoryInvokerTest
    {
        private ISenderFactoryInvoker GetInvoker()
        {
            return new DefaultSenderFactoryInvoker();
        }

        [Fact]
        public void CreateSender_throws_if_parameters_missing()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<ISenderFactory>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateSender(null, typeof(SimpleMessage), typeof(VoidType)));
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateSender(senderFactory, null, typeof(VoidType)));
            Assert.Throws<ArgumentNullException>(() => invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), null));
        }

        [Fact]
        public void CreateSender_succeeds_for_Message_and_VoidType()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<ISenderFactory>();

            // Act
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(VoidType));

            // Assert
            senderFactory.Received(1).CreateSender<SimpleMessage, VoidType>();
        }

        [Fact]
        public void CreateSender_succeeds_for_Message_and_string()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<ISenderFactory>();

            // Act
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(string));

            // Assert
            senderFactory.Received(1).CreateSender<SimpleMessage, string>();
        }

        [Fact]
        public void CreateSender_succeeds_for_Message_and_ComplexResult()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<ISenderFactory>();

            // Act
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));

            // Assert
            senderFactory.Received(1).CreateSender<SimpleMessage, SimpleResult>();
        }

        [Fact]
        public void CreateSender_succeeds_if_called_multiple_times_with_same_types()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<ISenderFactory>();

            // Act
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(SimpleResult));

            // Assert
            senderFactory.Received(3).CreateSender<SimpleMessage, SimpleResult>();
        }

        [Fact]
        public void CreateSender_succeeds_if_called_multiple_times_with_different_types()
        {
            // Arrange
            var invoker = GetInvoker();
            var senderFactory = Substitute.For<ISenderFactory>();

            // Act
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(string));
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleCommand), typeof(SimpleResult));
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleMessage), typeof(VoidType));
            invoker.InvokeCreateSender(senderFactory, typeof(SimpleCommand), typeof(SimpleResult));

            // Assert
            senderFactory.Received(1).CreateSender<SimpleMessage, string>();
            senderFactory.Received(1).CreateSender<SimpleMessage, VoidType>();
            senderFactory.Received(2).CreateSender<SimpleCommand, SimpleResult>();
        }
    }
}