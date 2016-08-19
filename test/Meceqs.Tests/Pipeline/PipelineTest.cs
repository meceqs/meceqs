using System;
using System.Threading.Tasks;
using Meceqs.Pipeline;
using Shouldly;
using Xunit;

namespace Meceqs.Tests.Pipeline
{
    public class PipelineTest
    {
        private IPipeline GetPipeline(FilterDelegate filterDelegate = null)
        {
            if (filterDelegate == null)
                filterDelegate = (ctx) => Task.CompletedTask;
            
            return new DefaultPipeline(filterDelegate, "pipeline");
        }

        [Fact]
        public void Ctor_throws_if_parameters_missing()
        {
            FilterDelegate filterDelegate = ctx => Task.CompletedTask;

            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(null, "pipeline"));
            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(filterDelegate, null));
            Should.Throw<ArgumentNullException>(() => new DefaultPipeline(filterDelegate, ""));
        }

        [Fact]
        public async Task Process_throws_if_context_missing()
        {
            var pipeline = GetPipeline();

            await Assert.ThrowsAsync<ArgumentNullException>(() => pipeline.ProcessAsync((FilterContext)null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => pipeline.ProcessAsync<string>((FilterContext)null));
        }

        [Fact]
        public async Task Process_invokes_pipeline_with_pipelineName()
        {
            var called = 0;
            FilterDelegate filter = (ctx) => {
                called++;
                ctx.PipelineName.ShouldBe("pipeline");
                return Task.CompletedTask; 
            };
            var pipeline = GetPipeline(filter);
            var context = TestObjects.FilterContext<SimpleMessage>();

            await pipeline.ProcessAsync(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task ProcessTResult_invokes_pipeline_with_PipelineName()
        {
            var called = 0;
            FilterDelegate filter = (ctx) => {
                called++;
                ctx.PipelineName.ShouldBe("pipeline");
                return Task.CompletedTask; 
            };
            var pipeline = GetPipeline(filter);
            var context = TestObjects.FilterContext<SimpleMessage>();

            await pipeline.ProcessAsync<string>(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task ProcessTResult_invokes_pipeline_with_ExpectedResultType()
        {
            var called = 0;
            FilterDelegate filter = (ctx) => {
                called++;
                ctx.ExpectedResultType.ShouldBe(typeof(string));
                return Task.CompletedTask; 
            };
            var pipeline = GetPipeline(filter);
            var context = TestObjects.FilterContext<SimpleMessage>();

            await pipeline.ProcessAsync<string>(context);

            called.ShouldBe(1);
        }

        [Fact]
        public async Task ProcessTResult_returns_result()
        {
            FilterDelegate filter = (ctx) => {
                ctx.Result = "result";
                return Task.CompletedTask; 
            };
            var pipeline = GetPipeline(filter);
            var context = TestObjects.FilterContext<SimpleMessage>();

            string result = await pipeline.ProcessAsync<string>(context);

            result.ShouldBe("result");
        }
    }
}