using System;
using System.Threading.Tasks;
using Meceqs.TypedHandling;

namespace Meceqs.Tests.Middleware.TypedHandling
{
    public class SimpleMessageIntHandler : IHandles<SimpleMessage, int>
    {
        private readonly int _result;

        public SimpleMessageIntHandler(int? result = null)
        {
            _result = result ?? 0;
        }

        public Task<int> HandleAsync(SimpleMessage msg, HandleContext context)
        {
            return Task.FromResult(_result);
        }
    }

    public class SimpleMessageSimpleResultHandler : IHandles<SimpleMessage, SimpleResult>
    {
        private readonly SimpleResult _result;

        public SimpleMessageSimpleResultHandler(SimpleResult result = null)
        {
            _result = result;
        }

        public Task<SimpleResult> HandleAsync(SimpleMessage msg, HandleContext context)
        {
            return Task.FromResult(_result);
        }
    }

    public class SimpleMessageNoResultHandler : IHandles<SimpleMessage>
    {
        private readonly Action _callback;

        public SimpleMessageNoResultHandler(Action callback = null)
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
        IHandles<SimpleCommand, SimpleResult>,
        IHandles<SimpleEvent, int>
    {
        private readonly Action<string> _callback;

        public MultipleMessagesHandler(Action<string> callback = null)
        {
            _callback = callback;
        }

        public Task HandleAsync(SimpleMessage msg, HandleContext context)
        {
            _callback?.Invoke("SimpleMessage/NoResult");
            return Task.CompletedTask;
        }

        public Task<SimpleResult> HandleAsync(SimpleCommand msg, HandleContext context)
        {
            _callback?.Invoke("SimpleCommand/SimpleResult");
            return Task.FromResult(new SimpleResult { Text = "result" });
        }

        public Task<int> HandleAsync(SimpleEvent msg, HandleContext context)
        {
            _callback?.Invoke("SimpleEvent/int");
            return Task.FromResult(1);
        }
    }
}