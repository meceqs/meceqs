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
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            _pipeline = pipeline;
            _pipelineName = pipelineName;
        }

        public Task ProcessAsync(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            EnrichContext(context);
            return _pipeline(context);
        }

        public async Task<TResult> ProcessAsync<TResult>(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            EnrichContext(context);

            context.ExpectedResultType = typeof(TResult);

            await _pipeline(context);

            return (TResult)context.Result;
        }

        private void EnrichContext(FilterContext context)
        {
            context.PipelineName = _pipelineName;
        }
    }
}