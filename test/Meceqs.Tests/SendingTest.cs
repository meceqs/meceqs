using System;
using System.Threading;
using Meceqs.Sending;
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

        public async void Test()
        {
            // assume we are in a message consumer who processes MyCommand
            MessageEnvelope<MyCommand> cmd = new MessageEnvelope<MyCommand>(Guid.NewGuid(), new MyCommand());

            // ... processing happens here ...

            // ... and results in an event!
            var evt = new MyEvent();

            // Now we want to send the event

            IMessageClient client = null;

            Guid id = Guid.NewGuid(); // Could come from somewhere else (e.g a client)

            string result = await client.CreateMessage(id, evt, CancellationToken.None /* e.g. HttpContext.RequestAborted */)

                // Copy tracing stuff from a source message
                .CorrelateWith(cmd)

                // Headers which will be part of the sent message
                .SetHeader("CreationReason", "DummyValue")

                // Properties which are used to send a message correctly - they can be used by decorators etc.
                .SetSendPartitionKey(evt.CustomerId)
                .SetSendProperty("ValueForSomeDocorator", "Asdf")

                // sends the message and assumes the result will be of the given type.
                .SendAsync<string>();
        }
    }
    
}