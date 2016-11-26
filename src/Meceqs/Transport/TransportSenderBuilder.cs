using System;
using Meceqs.Configuration;
using Meceqs.Pipeline;

namespace Meceqs.Transport
{
    public abstract class TransportSenderBuilder<TTransportSenderBuilder, TTransportSenderOptions>
        : ITransportSenderBuilder<TTransportSenderBuilder>
        where TTransportSenderBuilder : ITransportSenderBuilder<TTransportSenderBuilder>
        where TTransportSenderOptions : TransportSenderOptions
    {
        private string _pipelineName = MeceqsDefaults.SendPipelineName;
        private Action<IPipelineBuilder> _pipeline;

        /// <summary>
        /// Allows derived classes to modify the user-defined pipeline by adding additional
        /// middleware components at the beginning.
        /// </summary>
        protected Action<IPipelineBuilder> PipelineStartHook { get; set; }

        /// <summary>
        /// Allows derived classes to modify the user-defined pipeline by adding additional
        /// middleware components at the end.
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

        public TTransportSenderBuilder SetPipelineName(string pipelineName)
        {
            Check.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            _pipelineName = pipelineName;
            return Instance;
        }

        public TTransportSenderBuilder ConfigurePipeline(Action<IPipelineBuilder> pipeline)
        {
            Check.NotNull(pipeline, nameof(pipeline));

            _pipeline = pipeline;
            return Instance;
        }
    }
}