using System;
using System.Threading.Tasks;
using Meceqs.TypedHandling;

namespace Meceqs.Tests.Middleware.TypedHandling
{
    public class SimpleMessageIntHandler : IHandles<SimpleMessage, int>
    {
        private readonly int _response;

        public SimpleMessageIntHandler(int? response = null)
        {
            _response = response ?? 0;
        }

        public Task<int> HandleAsync(SimpleMessage msg, HandleContext context)
        {
            return Task.FromResult(_response);
        }
    }

    public class SimpleMessageSimpleResponseHandler : IHandles<SimpleMessage, SimpleResponse>
    {
        private readonly SimpleResponse _response;

        public SimpleMessageSimpleResponseHandler(SimpleResponse response = null)
        {
            _response = response;
        }

        public Task<SimpleResponse> HandleAsync(SimpleMessage msg, HandleContext context)
        {
            return Task.FromResult(_response);
        }
    }

    public class SimpleMessageNoResponseHandler : IHandles<SimpleMessage>
    {
        private readonly Action _callback;

        public SimpleMessageNoResponseHandler(Action callback = null)
        {
            _callback = callback;
        }

        public Task HandleAsync(SimpleMessage msg, HandleContext context)
        {
            _callback?.Invoke();

            return Task.CompletedTask;
        }
    }

    public class MultipleMessagesHandler :
        IHandles<SimpleMessage>,
        IHandles<SimpleCommand, SimpleResponse>,
        IHandles<SimpleEvent, int>
    {
        private readonly Action<string> _callback;

        public MultipleMessagesHandler(Action<string> callback = null)
        {
            _callback = callback;
        }

        public Task HandleAsync(SimpleMessage msg, HandleContext context)
        {
            _callback?.Invoke("SimpleMessage/NoResponse");
            return Task.CompletedTask;
        }

        public Task<SimpleResponse> HandleAsync(SimpleCommand msg, HandleContext context)
        {
            _callback?.Invoke("SimpleCommand/SimpleResponse");
            return Task.FromResult(new SimpleResponse { Text = "response" });
        }

        public Task<int> HandleAsync(SimpleEvent msg, HandleContext context)
        {
            _callback?.Invoke("SimpleEvent/int");
            return Task.FromResult(1);
        }
    }
}
