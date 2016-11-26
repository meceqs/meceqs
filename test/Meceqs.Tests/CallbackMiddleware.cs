using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Tests
{
    public class CallbackMiddleware
    {
        private readonly MessageDelegate _next;
        private readonly Action<MessageContext> _callback;

        public CallbackMiddleware(MessageDelegate next, Action<MessageContext> callback)
        {
            _next = next;
            _callback = callback;
        }

        public Task Invoke(MessageContext context)
        {
            if (_callback != null)
            {
                _callback(context);
                return Task.CompletedTask;
            }
            else
            {
                return _next(context);
            }
        }
    }

    public static class CallbackMiddlewarePipelineBuilderExtensions
    {
        public static void RunCallback(this IPipelineBuilder pipeline, Action<MessageContext> callback)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseMiddleware<CallbackMiddleware>(callback);
        }
    }
}