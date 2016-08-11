using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meceqs.Pipeline
{
    public class DefaultPipeline : IPipeline
    {
        private readonly FilterDelegate _pipeline;
        private readonly string _pipelineName;

        public DefaultPipeline(FilterDelegate pipeline, string pipelineName)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            _pipeline = pipeline;
            _pipelineName = pipelineName;
        }

        public Task ProcessAsync(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            EnrichContext(context);
            return _pipeline(context);
        }

        public async Task ProcessAsync(IList<FilterContext> contexts)
        {
            Check.NotNull(contexts, nameof(contexts));

            foreach (var context in contexts)
            {
                EnrichContext(context);
                await _pipeline(context);
            }
        }

        public async Task<TResult> ProcessAsync<TResult>(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            EnrichContext(context);

            context.ExpectedResultType = typeof(TResult);

            await _pipeline(context);

            return (TResult)context.Result;
        }

        public Task<TResult> ProcessAsync<TResult>(IList<FilterContext> contexts)
        {
            Check.NotNull(contexts, nameof(contexts));

            if (contexts.Count == 0)
            {
                return Task.FromResult(default(TResult));
            }
            else if (contexts.Count == 1)
            {
                return ProcessAsync<TResult>(contexts[0]);
            }
            else
            {
                throw new NotSupportedException($"'{nameof(ProcessAsync)}' with a result-type can only be called for a single envelope");
            }
        }

        private void EnrichContext(FilterContext context)
        {
            context.PipelineName = _pipelineName;
        }
    }
}