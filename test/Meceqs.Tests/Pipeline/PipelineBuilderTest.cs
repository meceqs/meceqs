using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineBuilderTest
    {
        private MiddlewareDelegate GetPipeline(Action<PipelineBuilder> options = null)
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var optionsInstance = new PipelineBuilder("foo");
            options?.Invoke(optionsInstance);

            return optionsInstance.BuildPipeline(serviceProvider);
        }

        [Fact]
        public void Pipeline_is_not_null_if_no_middleware_registered()
        {
            var pipeline = GetPipeline();

            Assert.NotNull(pipeline);
        }

        [Fact]
        public void Throws_if_no_terminating_middleware_registered()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var pipeline = GetPipeline(options =>
            {
                options.Use((ctx, next) => next());
            });

            Should.Throw<InvalidOperationException>(async () => await pipeline(context));
        }

        [Fact]
        public void Calls_multiple_middleware_and_throws_if_no_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var called1 = 0;
            var called2 = 0;

            var pipeline = GetPipeline(options =>
            {
                options.Use((ctx, next) => { called1++; return next(); });
                options.Use((ctx, next) => { called2++; return next(); });
            });

            Should.Throw<InvalidOperationException>(async () => await pipeline(context));

            called1.ShouldBe(1);
            called2.ShouldBe(1);
        }

        [Fact]
        public async Task Calls_multiple_middleware_with_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();

            var called1 = 0;
            var called2 = 0;
            var called3 = 0;

            var pipeline = GetPipeline(options =>
            {
                options.Use((ctx, next) => { called1++; return next(); });
                options.Use((ctx, next) => { called2++; return next(); });
                options.Run(ctx => { called3++; return Task.CompletedTask; });
            });

            await pipeline(context);

            called1.ShouldBe(1);
            called2.ShouldBe(1);
            called3.ShouldBe(1);
        }


        [Fact]
        public async Task Calls_one_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var called = 0;

            var pipeline = GetPipeline(options =>
            {
                options.Run(ctx => { called++; return Task.CompletedTask; });
            });

            await pipeline(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task Doesnt_call_second_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();

            var called1 = 0;
            var called2 = 0;

            var pipeline = GetPipeline(options =>
            {
                options.Run(ctx => { called1++; return Task.CompletedTask; });
                options.Run(ctx => { called2++; return Task.CompletedTask; });
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
            var called2 = 0;

            var pipeline = GetPipeline(options =>
            {
                options.Run(ctx => { called1++; return Task.CompletedTask; });
                options.Use((ctx, next) => { called2++; return next(); });
            });

            await pipeline(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }
    }
}