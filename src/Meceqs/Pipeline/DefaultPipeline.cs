using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Meceqs.Pipeline
{
    public class DefaultPipeline : IPipeline
    {
        private readonly FilterDelegate _pipeline;
        private readonly string _pipelineName;
        private readonly ILogger _logger;

        public DefaultPipeline(FilterDelegate pipeline, string pipelineName, ILoggerFactory loggerFactory)
        {
            Check.NotNull(pipeline, nameof(pipeline));
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _pipeline = pipeline;
            _pipelineName = pipelineName;
            _logger = loggerFactory.CreateLogger<DefaultPipeline>();
        }

        public Task ProcessAsync(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            return ExecutePipeline(context);
        }

        public async Task<TResult> ProcessAsync<TResult>(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            context.ExpectedResultType = typeof(TResult);

            await ExecutePipeline(context);

            return (TResult)context.Result;
        }

        private Task ExecutePipeline(FilterContext context)
        {
            context.PipelineName = _pipelineName;

            _logger.ExecutingPipeline(context);

            return _pipeline(context);
        }
    }
}