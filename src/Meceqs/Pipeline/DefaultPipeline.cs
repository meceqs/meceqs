using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Meceqs.Pipeline
{
    public class DefaultPipeline : IPipeline
    {
        private readonly MiddlewareDelegate _pipeline;
        private readonly ILogger _logger;
        private readonly IMessageContextEnricher _messageContextEnricher;

        public string Name { get; }

        public DefaultPipeline(
            MiddlewareDelegate pipeline,
            string pipelineName,
            ILoggerFactory loggerFactory,
            IMessageContextEnricher messageContextEnricher)
        {
            Guard.NotNull(pipeline, nameof(pipeline));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Guard.NotNull(loggerFactory, nameof(loggerFactory));

            _pipeline = pipeline;
            _logger = loggerFactory.CreateLogger<DefaultPipeline>();
            _messageContextEnricher = messageContextEnricher;

            Name = pipelineName;
        }

        public Task InvokeAsync(MessageContext context)
        {
            Guard.NotNull(context, nameof(context));

            return ExecutePipeline(context);
        }

        private Task ExecutePipeline(MessageContext context)
        {
            // Give frameworks a chance to add additional properties to the context before
            // the pipeline is executed.
            _messageContextEnricher?.EnrichMessageContext(context);

            _logger.ExecutingPipeline(context);

            context.Envelope.EnsureValid();

            return _pipeline(context);
        }
    }
}