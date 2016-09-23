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
                // Envelope is enforced in the FilterContext constructor so we don't need a nice error message here.
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Envelope)}");
            }

            if (context.Message == null)
            {
                throw new ArgumentNullException(
                    $"The envelope does not contain a message object. You should call '{nameof(Envelope)}.{nameof(Envelope.EnsureValid)}' " +
                    $"before invoking the pipeline to make sure this error already throws when the envelope is created.",
                    $"{nameof(context)}.{nameof(context.Message)}"
                );
            }

            if (context.MessageType == null)
            {
                // This is a shortcut property and the user can't influence it so we don't need a nice error message here.
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.MessageType)}");
            }

            if (string.IsNullOrWhiteSpace(context.PipelineName))
            {
                // This is enforced in the constructor of this class so this should not happen - we test it here anyway.
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.PipelineName)}");
            }

            if (context.RequestServices == null)
            {
                throw new ArgumentNullException(
                    $"The {nameof(FilterContext)} has not been configured with a service provider. " +
                    $"You can set it by either calling '{nameof(Meceqs.Consuming.IFluentConsumer.SetRequestServices)}' on " +
                    $"a filter context builder ('{nameof(Meceqs.Consuming.IFluentConsumer)}' or '{nameof(Meceqs.Sending.IFluentSender)}') " +
                    $"or by implementing a '{nameof(IFilterContextEnricher)}' that automatically sets the service provider " +
                    $"for every request.",
                    $"{nameof(context)}.{nameof(context.RequestServices)}"
                );
            }
        }
    }
}