using System;
using Meceqs.Sending;
using NSubstitute;
using Xunit;

namespace Meceqs.Tests
{
    public class SendingTest
    {

        public class MyCommand : ICommand
        {
            public Guid CustomerId { get; set; }
        }

        public class MyEvent : IEvent
        {
            public Guid CustomerId { get; set; }
        }

        // public class AspNetMessageClient
        // {
        //     private readonly IMessageClient _messageClient;

        //     public AspNetMessageClient(IMessageClient messageClient)
        //     {
        //         if (messageClient == null)
        //             throw new ArgumentNullException(nameof(messageClient));

        //         _messageClient = messageClient;
        //     }

        //     public ISendContextBuilder CreateMessage(Guid messageId, IMessage message, HttpContext httpContext)
        //     {
        //         if (httpContext == null)
        //             throw new ArgumentNullException(nameof(httpContext));

        //         var builder = _messageClient.CreateMessage(messageId, message, httpContext.RequestAborted);

        //         builder.SendContext.Message.CorrelationId = httpContext.Request.TraceIdentifier;
        //     }
        // }

        private MessageEnvelope GetEnvelope(IMessage msg)
        {
            return new MessageEnvelope(Guid.NewGuid(), msg);
        }

        private IMessageClient GetMessageClient(ISendTransport sender)
        {
            sender = sender ?? Substitute.For<ISendTransport>();
            return new DefaultMessageClient(sender);
        }

        [Fact]
        public async void Test()
        {
            // assume we are in a message handler who processes MyCommand
            var sourceCmd = GetEnvelope(new MyCommand());

            // ... processing happens here ...

            // ... and results in an event!
            var evt = new MyEvent();
            var id = Guid.NewGuid(); // Could come from somewhere else (e.g a client)

            // Now we want to send the event

            var sender = Substitute.For<ISendTransport>();
            var messageClient = GetMessageClient(sender); // gets injected by DI 

            string result = await messageClient.ForEvent(id, evt, sourceCmd)

                //.SetCancellationToken(HttpContext.RequestAborted) // a decorator would do that

                // Copy tracing stuff from a source message
                //.CorrelateWith(cmd) // if not specified in the Create*-method

                // Headers which will be part of the sent message
                .SetHeader("CreationReason", "DummyValue")

                // Properties which are used to send a message correctly - they can be used by decorators etc.
                .SetSendPartitionKey(evt.CustomerId)
                .SetSendProperty("ValueForSomeDocorator", "Asdf")

                // sends the message and assumes the result will be of the given type.
                .SendAsync<string>();

            // Assert

            await sender.ReceivedWithAnyArgs(1).SendAsync<MyCommand, string>(Arg.Any<SendContext<MyCommand>>());
        }
    }
}