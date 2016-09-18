using System;
using Meceqs.Pipeline;
using Meceqs.TypedHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public abstract class TransportSenderBuilder<TTransportSenderBuilder, TTransportSenderOptions>
        : ITransportSenderBuilder<TTransportSenderBuilder>
        where TTransportSenderBuilder : ITransportSenderBuilder<TTransportSenderBuilder>
        where TTransportSenderOptions : TransportSenderOptions
    {
        private Action<TTransportSenderOptions> _senderOptions;

        private string _pipelineName;
        private Action<IPipelineBuilder> _pipeline;

        /// <summary>
        /// Allows derived classes to modify the user-defined pipeline by adding additional filters
        /// at the beginning.
        /// </summary>
        protected Action<IPipelineBuilder> PipelineStartHook { get; set; }

        /// <summary>
        /// Allows derived classes to modify the user-defined pipeline by adding additional filters
        /// at the end.
        /// </summary>
        protected Action<IPipelineBuilder> PipelineEndHook { get; set; }

        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        public abstract TTransportSenderBuilder Instance { get; }

        public Action<TTransportSenderOptions> GetSenderOptions() => _senderOptions;
        public string GetPipelineName() => _pipelineName;

        public Action<IPipelineBuilder> GetPipeline()
        {
            var pipeline = PipelineStartHook + _pipeline + PipelineEndHook;

            if (pipeline == null)
            {
                throw new MeceqsException(
                    $"No pipeline was configured. You can either use TypedHandling by calling " +
                    $"'{nameof(UseTypedHandling)}' or you can configure a custom pipeline " +
                    $"by calling '{nameof(ConfigurePipeline)}'.");
            }

            return pipeline;
        }

        public TTransportSenderBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline)
        {
            return ConfigurePipeline(null, pipeline);
        }

        public TTransportSenderBuilder ConfigurePipeline(string pipelineName, Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            _pipelineName = pipelineName;
            _pipeline = pipeline;
            return Instance;
        }

        public TTransportSenderBuilder UseTypedHandling(Action<TypedHandlingOptions> options)
        {
            Check.NotNull(options, nameof(options));

            var handlingOptions = new TypedHandlingOptions();
            options(handlingOptions);

            return UseTypedHandling(handlingOptions);
        }

        public TTransportSenderBuilder UseTypedHandling(TypedHandlingOptions options)
        {
            Check.NotNull(options, nameof(options));

            // Add the filter to the end of the pipeline.
            PipelineEndHook = pipeline =>
            {
                pipeline.RunTypedHandling(options);
            };

            return Instance;
        }
    }
}