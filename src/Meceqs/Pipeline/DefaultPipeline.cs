using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Meceqs.Pipeline
{
    public class DefaultPipeline : IPipeline
    {
        private readonly FilterDelegate _pipeline;
        private readonly ILogger _logger;
        private readonly IFilterContextEnricher _filterContextEnricher;

        public string Name { get; }

        public DefaultPipeline(
            FilterDelegate pipeline,
            string pipelineName,
            ILoggerFactory loggerFactory,
            IFilterContextEnricher filterContextEnricher)
        {
            Check.NotNull(pipeline, nameof(pipeline));
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _pipeline = pipeline;
            _logger = loggerFactory.CreateLogger<DefaultPipeline>();
            _filterContextEnricher = filterContextEnricher;

            Name = pipelineName;
        }

        public Task InvokeAsync(FilterContext context)
        {
            Check.NotNull(context, nameof(context));

            return ExecutePipeline(context);
        }

        private Task ExecutePipeline(FilterContext context)
        {
            // Give frameworks a chance to add additional properties to the context before
            // the pipeline is executed.
            _filterContextEnricher?.EnrichFilterContext(context);

            _logger.ExecutingPipeline(context);

            ValidateFilterContext(context);

            return _pipeline(context);
        }

        private void ValidateFilterContext(FilterContext context)
        {
            if (context.Envelope == null)
            {
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Envelope)}");
            }

            context.Envelope.EnsureValid();

            if (string.IsNullOrWhiteSpace(context.PipelineName))
            {
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.PipelineName)}");
            }

            if (context.RequestServices == null)
            {
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.RequestServices)}");
            }
        }
    }
}