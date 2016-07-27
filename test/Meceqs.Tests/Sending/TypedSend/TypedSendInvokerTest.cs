using System;
using System.Threading;
using System.Threading.Tasks;
using Meceqs.Sending.TypedSend;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests.Sending.TypedSend
{
    public class TypedSendInvokerTest
    {
        private class SimpleMessageStringSender : ISender<SimpleMessage, string>
        {
            private readonly string _result;

            public SimpleMessageStringSender(string result)
            {
                _result = result;
            }

            public Task<string> SendAsync(MessageContext<SimpleMessage> context)
            {
                return Task.FromResult(_result);
            }
        }

        private ITypedSendInvoker GetInvoker()
        {
            return new DefaultTypedSendInvoker();
        }

        private MessageContext GetMessageContext<TMessage>()
            where TMessage : class, IMessage, new()
        {
            var envelope = TestObjects.Envelope<TMessage>();
            var messageContextData = new MessageContextData();

            return new MessageContext<TMessage>(envelope, messageContextData, CancellationToken.None);
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

        [Fact]
        public async Task SendAsync_succeeds()
        {
            // Arrange
            
            var invoker = GetInvoker();
            
            var sender = new SimpleMessageStringSender("result");
            var messageContext = GetMessageContext<SimpleMessage>();

            // Act
            string result = await invoker.InvokeSendAsync<string>(sender, messageContext);

            // Assert
            Assert.Equal("result", result);
        }
    }
}