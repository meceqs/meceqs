using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineTest
    {
        private IPipeline GetPipeline(MiddlewareDelegate middleware = null)
        {
            if (middleware == null)
                middleware = (ctx) => Task.CompletedTask;

            return new DefaultPipeline(middleware, "pipeline", Substitute.For<ILoggerFactory>(), null);
        }

        [Fact]
        public void Ctor_throws_if_parameters_missing()
        {
            MiddlewareDelegate middleware = ctx => Task.CompletedTask;
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var messageContextEnricher = Substitute.For<IMessageContextEnricher>();

            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(null, "pipeline", loggerFactory, messageContextEnricher));
            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(middleware, null, loggerFactory, messageContextEnricher));
            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(middleware, "pipeline", null, messageContextEnricher));
        }

        [Fact]
        public void Ctor_DoesNotThrow_if_MessageContextEnricher_missing()
        {
            MiddlewareDelegate pipeline = ctx => Task.CompletedTask;
            var loggerFactory = Substitute.For<ILoggerFactory>();

            new DefaultPipeline(pipeline, "pipeline", loggerFactory, null);
        }

        [Fact]
        public async Task Invoke_throws_if_context_missing()
        {
            var pipeline = GetPipeline();

            await Assert.ThrowsAsync<ArgumentNullException>(() => pipeline.InvokeAsync((MessageContext)null));
        }

        [Fact]
        public async Task Invoke_calls_middleware_with_pipelineName()
        {
            var called = 0;
            MiddlewareDelegate middleware = (ctx) => {
                called++;
                ctx.PipelineName.ShouldBe("pipeline");
                return Task.CompletedTask;
            };
            var pipeline = GetPipeline(middleware);
            var context = TestObjects.MessageContext<SimpleMessage>();

            await pipeline.InvokeAsync(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task Invoke_calls_middleware_with_ExpectedResponseType()
        {
            var called = 0;
            MiddlewareDelegate middleware = (ctx) => {
                called++;
                ctx.ExpectedResponseType.ShouldBe(typeof(string));
                return Task.CompletedTask;
            };
            var pipeline = GetPipeline(middleware);
            var context = TestObjects.MessageContext<SimpleMessage>(responseType: typeof(string));

            await pipeline.InvokeAsync(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task Invoke_with_expectedResponseType_returns_response()
        {
            MiddlewareDelegate middleware = (ctx) => {
                ctx.Response = "response";
                return Task.CompletedTask;
            };
            var pipeline = GetPipeline(middleware);
            var context = TestObjects.MessageContext<SimpleMessage>(responseType: typeof(string));

            await pipeline.InvokeAsync(context);

            string response = (string)context.Response;

            response.ShouldBe("response");
        }
    }
}