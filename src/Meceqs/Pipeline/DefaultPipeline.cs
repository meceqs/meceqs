using System;
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
            Check.NotNull(pipeline, nameof(pipeline));
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _pipeline = pipeline;
            _logger = loggerFactory.CreateLogger<DefaultPipeline>();
            _messageContextEnricher = messageContextEnricher;

            Name = pipelineName;
        }

        public Task InvokeAsync(MessageContext context)
        {
            Check.NotNull(context, nameof(context));

            return ExecutePipeline(context);
        }

        private Task ExecutePipeline(MessageContext context)
        {
            // Give frameworks a chance to add additional properties to the context before
            // the pipeline is executed.
            _messageContextEnricher?.EnrichMessageContext(context);

            _logger.ExecutingPipeline(context);

            ValidateMessageContext(context);

            return _pipeline(context);
        }

        private void ValidateMessageContext(MessageContext context)
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