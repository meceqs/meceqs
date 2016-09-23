using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Tests
{
    public class CallbackFilter
    {
        private readonly FilterDelegate _next;
        private readonly Action<FilterContext> _callback;

        public CallbackFilter(FilterDelegate next, Action<FilterContext> callback)
        {
            _next = next;
            _callback = callback;
        }

        public Task Invoke(FilterContext context)
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

    public static class CallbackFilterPipelineBuilderExtensions
    {
        public static void RunCallback(this IPipelineBuilder pipeline, Action<FilterContext> callback)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            pipeline.UseFilter<CallbackFilter>(callback);
        }
    }
}