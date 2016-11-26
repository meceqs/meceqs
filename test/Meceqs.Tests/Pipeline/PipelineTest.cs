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
        private IPipeline GetPipeline(MessageDelegate messageDelegate = null)
        {
            if (messageDelegate == null)
                messageDelegate = (ctx) => Task.CompletedTask;

            return new DefaultPipeline(messageDelegate, "pipeline", Substitute.For<ILoggerFactory>(), null);
        }

        [Fact]
        public void Ctor_throws_if_parameters_missing()
        {
            MessageDelegate messageDelegate = ctx => Task.CompletedTask;
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var messageContextEnricher = Substitute.For<IMessageContextEnricher>();

            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(null, "pipeline", loggerFactory, messageContextEnricher));
            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(messageDelegate, null, loggerFactory, messageContextEnricher));
            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(messageDelegate, "pipeline", null, messageContextEnricher));
        }

        [Fact]
        public void Ctor_DoesNotThrow_if_MessageContextEnricher_missing()
        {
            MessageDelegate pipeline = ctx => Task.CompletedTask;
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
            MessageDelegate middleware = (ctx) => {
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
        public async Task Invoke_calls_middleware_with_ExpectedResultType()
        {
            var called = 0;
            MessageDelegate middleware = (ctx) => {
                called++;
                ctx.ExpectedResultType.ShouldBe(typeof(string));
                return Task.CompletedTask;
            };
            var pipeline = GetPipeline(middleware);
            var context = TestObjects.MessageContext<SimpleMessage>(resultType: typeof(string));

            await pipeline.InvokeAsync(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task Invoke_with_expectedResultType_returns_result()
        {
            MessageDelegate middleware = (ctx) => {
                ctx.Result = "result";
                return Task.CompletedTask;
            };
            var pipeline = GetPipeline(middleware);
            var context = TestObjects.MessageContext<SimpleMessage>(resultType: typeof(string));

            await pipeline.InvokeAsync(context);

            string result = (string)context.Result;

            result.ShouldBe("result");
        }
    }
}