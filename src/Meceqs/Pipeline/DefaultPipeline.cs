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

        public Task ProcessAsync(IList<FilterContext> contexts)
        {
            return ProcessAsync<VoidType>(contexts);
        }

        public async Task<TResult> ProcessAsync<TResult>(IList<FilterContext> contexts)
        {
            // TODO @cweiss does this make sense?

            if (contexts.Count == 0)
            {
                return await Task.FromResult(default(TResult));
            }
            else if (contexts.Count == 1)
            {
                return await ProcessAsync<TResult>(contexts[0]);
            }
            else
            {
                if (typeof(TResult) != typeof(VoidType))
                    throw new InvalidOperationException("SendAsync with many contexts can only be used with return-type 'VoidType'");

                foreach (var context in contexts)
                {
                    await ProcessAsync<TResult>(context);
                }

                return await Task.FromResult(default(TResult));
            }
        }

        private async Task<TResult> ProcessAsync<TResult>(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            context.PipelineName = _pipelineName;
            context.ExpectedResultType = typeof(TResult);

            await _pipeline(context);

            return (TResult)context.Result;
        }
    }
}