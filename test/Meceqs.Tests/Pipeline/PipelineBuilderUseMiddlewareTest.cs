using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineBuilderUseMiddlewareTest
    {
        private class CallbackMiddleware
        {
            private readonly MessageDelegate _next;
            private readonly Action _callback;

            public CallbackMiddleware(MessageDelegate next, Action callback)
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

            public TerminatingCallbackMiddleware(MessageDelegate next, Action callback)
            {
                _callback = callback;
            }

            public Task Invoke(MessageContext context)
            {
                _callback();
                return Task.CompletedTask;
            }
        }

        private IPipelineBuilder GetPipelineBuilder()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();

            return new DefaultPipelineBuilder(serviceProvider, Substitute.For<ILoggerFactory>(), null);
        }

        [Fact]
        public async Task Calls_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            Action callback1 = () => called1++;
            builder.UseMiddleware<TerminatingCallbackMiddleware>(callback1);

            var pipeline = builder.Build("pipeline");

            await pipeline.InvokeAsync(context);

            called1.ShouldBe(1);
        }

        [Fact]
        public async Task Calls_multiple_middleware_components()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            Action callback1 = () => called1++;
            builder.UseMiddleware<CallbackMiddleware>(callback1);

            var called2 = 0;
            Action callback2 = () => called2++;
            builder.UseMiddleware<CallbackMiddleware>(callback2);

            var called3 = 0;
            Action callback3 = () => called3++;
            builder.UseMiddleware<TerminatingCallbackMiddleware>(callback3);

            var pipeline = builder.Build("pipeline");

            await pipeline.InvokeAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(1);
            called3.ShouldBe(1);
        }

        [Fact]
        public async Task Doesnt_call_second_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            Action callback1 = () => called1++;
            builder.UseMiddleware<TerminatingCallbackMiddleware>(callback1);

            var called2 = 0;
            Action callback2 = () => called2++;
            builder.UseMiddleware<TerminatingCallbackMiddleware>(callback2);

            var pipeline = builder.Build("pipeline");

            await pipeline.InvokeAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }

        [Fact]
        public async Task Doesnt_call_regular_middleware_after_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            Action callback1 = () => called1++;
            builder.UseMiddleware<TerminatingCallbackMiddleware>(callback1);

            var called2 = 0;
            Action callback2 = () => called2++;
            builder.UseMiddleware<CallbackMiddleware>(callback2);

            var pipeline = builder.Build("pipeline");

            await pipeline.InvokeAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }
    }
}