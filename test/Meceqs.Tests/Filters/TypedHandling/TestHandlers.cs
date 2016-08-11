using System;
using System.Threading.Tasks;
using Meceqs.Filters.TypedHandling;

namespace Meceqs.Tests.Filters.TypedHandling
{
    public class SimpleMessageIntHandler : IHandles<SimpleMessage, int>
    {
        private readonly int _result;

        public SimpleMessageIntHandler(int? result = null)
        {
            _result = result ?? 0;
        }

        public Task<int> HandleAsync(HandleContext<SimpleMessage> context)
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

        public Task<SimpleResult> HandleAsync(HandleContext<SimpleMessage> context)
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

        public Task HandleAsync(HandleContext<SimpleMessage> context)
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

        public Task HandleAsync(HandleContext<SimpleMessage> context)
        {
            _callback?.Invoke("SimpleMessage/NoResult");
            return Task.CompletedTask;
        }

        public Task<SimpleResult> HandleAsync(HandleContext<SimpleCommand> context)
        {
            _callback?.Invoke("SimpleCommand/SimpleResult");
            return Task.FromResult(new SimpleResult { Text = "result" });
        }

        public Task<int> HandleAsync(HandleContext<SimpleEvent> context)
        {
            _callback?.Invoke("SimpleEvent/int");
            return Task.FromResult(1);
        }
    }
}