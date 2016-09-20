using System;
using Meceqs.Pipeline;

namespace Meceqs.Transport
{
    public abstract class TransportSenderBuilder<TTransportSenderBuilder, TTransportSenderOptions>
        : ITransportSenderBuilder<TTransportSenderBuilder>
        where TTransportSenderBuilder : ITransportSenderBuilder<TTransportSenderBuilder>
        where TTransportSenderOptions : TransportSenderOptions
    {
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

        protected Action<TTransportSenderOptions> SenderOptions { get; set; }

        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        public abstract TTransportSenderBuilder Instance { get; }

        public Action<TTransportSenderOptions> GetSenderOptions() => SenderOptions;
        public string GetPipelineName() => _pipelineName;

        public Action<IPipelineBuilder> GetPipeline()
        {
            var pipeline = PipelineStartHook + _pipeline + PipelineEndHook;

            if (pipeline == null)
            {
                throw new MeceqsException(
                    $"No pipeline was configured. Configure a custom pipeline " +
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
    }
}