using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineUseMiddlewareTest
    {
        private class CallbackMiddleware
        {
            private readonly MiddlewareDelegate _next;
            private readonly Action _callback;

            public CallbackMiddleware(MiddlewareDelegate next, Action callback)
            {
                _next = next;
                _callback = callback;
            }

            public Task Invoke(MessageContext context)
            {
                _callback();
                return _next(context);
            }
        }

        private class TerminatingCallbackMiddleware
        {
            private readonly Action _callback;

            public TerminatingCallbackMiddleware(MiddlewareDelegate next, Action callback)
            {
                _callback = callback;
            }

            public Task Invoke(MessageContext context)
            {
                _callback();
                return Task.CompletedTask;
            }
        }

        private MiddlewareDelegate GetPipeline(Action<PipelineOptions> options = null)
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var optionsInstance = new PipelineOptions();
            options?.Invoke(optionsInstance);

            return optionsInstance.BuildPipelineDelegate(serviceProvider);
        }

        [Fact]
        public async Task Calls_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();

            var called1 = 0;
            Action callback1 = () => called1++;

            var pipeline = GetPipeline(options =>
            {
                options.UseMiddleware<TerminatingCallbackMiddleware>(callback1);
            });

            await pipeline(context);

            called1.ShouldBe(1);
        }

        [Fact]
        public async Task Calls_multiple_middleware_components()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();

            var called1 = 0;
            Action callback1 = () => called1++;

            var called2 = 0;
            Action callback2 = () => called2++;

            var called3 = 0;
            Action callback3 = () => called3++;

            var pipeline = GetPipeline(options =>
            {
                options.UseMiddleware<CallbackMiddleware>(callback1);
                options.UseMiddleware<CallbackMiddleware>(callback2);
                options.UseMiddleware<TerminatingCallbackMiddleware>(callback3);
            });
            
            await pipeline(context);

            called1.ShouldBe(1);
            called2.ShouldBe(1);
            called3.ShouldBe(1);
        }

        [Fact]
        public async Task Doesnt_call_second_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();

            var called1 = 0;
            Action callback1 = () => called1++;

            var called2 = 0;
            Action callback2 = () => called2++;

            var pipeline = GetPipeline(options =>
            {
                options.UseMiddleware<TerminatingCallbackMiddleware>(callback1);
                options.UseMiddleware<TerminatingCallbackMiddleware>(callback2);
            });

            await pipeline(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }

        [Fact]
        public async Task Doesnt_call_regular_middleware_after_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();

            var called1 = 0;
            Action callback1 = () => called1++;

            var called2 = 0;
            Action callback2 = () => called2++;

            var pipeline = GetPipeline(options =>
            {
                options.UseMiddleware<TerminatingCallbackMiddleware>(callback1);
                options.UseMiddleware<CallbackMiddleware>(callback2);
            });

            await pipeline(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }
    }
}