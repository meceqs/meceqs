using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineBuilderTest
    {
        private IPipelineBuilder GetPipelineBuilder()
        {
            return new DefaultPipelineBuilder(Substitute.For<IServiceProvider>(), "pipeline");
        }

        [Fact]
        public void Ctor_throws_if_parameters_missing()
        {
            Should.Throw<ArgumentNullException>(() => new DefaultPipelineBuilder(null, "pipeline"));
            Should.Throw<ArgumentNullException>(() => new DefaultPipelineBuilder(Substitute.For<IServiceProvider>(), null));
        }

        [Fact]
        public void Throws_if_no_filter_registered()
        {
            var context = TestObjects.FilterContext<SimpleMessage>();
            var builder = GetPipelineBuilder();
            var pipeline = builder.Build();

            Should.Throw<InvalidOperationException>(async () => await pipeline.ProcessAsync(context));
        }

        [Fact]
        public void Throws_if_no_terminating_filter_registered()
        {
            var context = TestObjects.FilterContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            builder.Use((ctx, next) => next());

            var pipeline = builder.Build();

            Should.Throw<InvalidOperationException>(async () => await pipeline.ProcessAsync(context));
        }

        [Fact]
        public void Calls_multiple_filters_and_throws_if_no_terminating_filter()
        {
            var context = TestObjects.FilterContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            builder.Use((ctx, next) => { called1++; return next(); });

            var called2 = 0;
            builder.Use((ctx, next) => { called2++; return next(); });

            var pipeline = builder.Build();

            Should.Throw<InvalidOperationException>(async () => await pipeline.ProcessAsync(context));
            
            called1.ShouldBe(1);
            called2.ShouldBe(1);
        }

        [Fact]
        public async Task Calls_multiple_filters_with_terminating_filter()
        {
            var context = TestObjects.FilterContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            builder.Use((ctx, next) => { called1++; return next(); });

            var called2 = 0;
            builder.Use((ctx, next) => { called2++; return next(); });

            var called3 = 0;
            builder.Run(ctx => { called3++; return Task.CompletedTask; });

            var pipeline = builder.Build();

            await pipeline.ProcessAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(1);
            called3.ShouldBe(1);
        }


        [Fact]
        public async Task Calls_one_terminating_filter()
        {
            var context = TestObjects.FilterContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called = 0;
            builder.Run(ctx => { called++; return Task.CompletedTask; });

            var pipeline = builder.Build();

            await pipeline.ProcessAsync(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task Doesnt_call_second_terminating_filter()
        {
            var context = TestObjects.FilterContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            builder.Run(ctx => { called1++; return Task.CompletedTask; });

            var called2 = 0;
            builder.Run(ctx => { called2++; return Task.CompletedTask; });

            var pipeline = builder.Build();

            await pipeline.ProcessAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }

        [Fact]
        public async Task Doesnt_call_regular_filter_after_terminating_filter()
        {
            var context = TestObjects.FilterContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            builder.Run(ctx => { called1++; return Task.CompletedTask; });

            var called2 = 0;
            builder.Use((ctx, next) => { called2++; return next(); });

            var pipeline = builder.Build();

            await pipeline.ProcessAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }
    }
}