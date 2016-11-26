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
    public class PipelineBuilderTest
    {
        private IPipelineBuilder GetPipelineBuilder()
        {
            var builder = new DefaultPipelineBuilder(
                Substitute.For<IServiceProvider>(),
                Substitute.For<ILoggerFactory>(),
                null);

            return builder;
        }

        [Fact]
        public void Ctor_throws_if_parameters_missing()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var messageContextEnricher = Substitute.For<IMessageContextEnricher>();

            Should.Throw<ArgumentNullException>(() => new DefaultPipelineBuilder(null, loggerFactory, messageContextEnricher));
            Should.Throw<ArgumentNullException>(() => new DefaultPipelineBuilder(serviceProvider, null, messageContextEnricher));
        }

        [Fact]
        public void Ctor_DoesNotThrow_if_MessageContexEnricher_missing()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var loggerFactory = Substitute.For<ILoggerFactory>();

            new DefaultPipelineBuilder(serviceProvider, loggerFactory, null);
        }

        [Fact]
        public void Throws_if_no_middleware_registered()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();
            var pipeline = builder.Build("pipeline");

            Should.Throw<InvalidOperationException>(async () => await pipeline.InvokeAsync(context));
        }

        [Fact]
        public void Throws_if_no_terminating_middleware_registered()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            builder.Use((ctx, next) => next());

            var pipeline = builder.Build("pipeline");

            Should.Throw<InvalidOperationException>(async () => await pipeline.InvokeAsync(context));
        }

        [Fact]
        public void Calls_multiple_middleware_and_throws_if_no_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            builder.Use((ctx, next) => { called1++; return next(); });

            var called2 = 0;
            builder.Use((ctx, next) => { called2++; return next(); });

            var pipeline = builder.Build("pipeline");

            Should.Throw<InvalidOperationException>(async () => await pipeline.InvokeAsync(context));

            called1.ShouldBe(1);
            called2.ShouldBe(1);
        }

        [Fact]
        public async Task Calls_multiple_middleware_with_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            builder.Use((ctx, next) => { called1++; return next(); });

            var called2 = 0;
            builder.Use((ctx, next) => { called2++; return next(); });

            var called3 = 0;
            builder.Run(ctx => { called3++; return Task.CompletedTask; });

            var pipeline = builder.Build("pipeline");

            await pipeline.InvokeAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(1);
            called3.ShouldBe(1);
        }


        [Fact]
        public async Task Calls_one_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called = 0;
            builder.Run(ctx => { called++; return Task.CompletedTask; });

            var pipeline = builder.Build("pipeline");

            await pipeline.InvokeAsync(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task Doesnt_call_second_terminating_middleware()
        {
            var context = TestObjects.MessageContext<SimpleMessage>();
            var builder = GetPipelineBuilder();

            var called1 = 0;
            builder.Run(ctx => { called1++; return Task.CompletedTask; });

            var called2 = 0;
            builder.Run(ctx => { called2++; return Task.CompletedTask; });

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
            builder.Run(ctx => { called1++; return Task.CompletedTask; });

            var called2 = 0;
            builder.Use((ctx, next) => { called2++; return next(); });

            var pipeline = builder.Build("pipeline");

            await pipeline.InvokeAsync(context);

            called1.ShouldBe(1);
            called2.ShouldBe(0);
        }
    }
}