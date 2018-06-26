using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using Meceqs.Pipeline;

namespace Meceqs.Transport
{
    public abstract class SendTransportBuilder<TSendTransportBuilder, TSendTransportOptions> : TransportPipelineBuilder
        where TSendTransportBuilder : SendTransportBuilder<TSendTransportBuilder, TSendTransportOptions>
        where TSendTransportOptions : SendTransportOptions
    {
        /// <summary>
        /// This property is only necessary to support the builder pattern with
        /// the generic arguments from this type.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected abstract TSendTransportBuilder Instance { get; }

        protected SendTransportBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
            : base(meceqsBuilder, pipelineName ?? MeceqsDefaults.SendPipelineName)
        {
            Services.Configure<TSendTransportOptions>(PipelineName, PipelineConfiguration);
        }

        public TSendTransportBuilder Configure(Action<TSendTransportOptions> options)
        {
            if (options != null)
            {
                Services.Configure(PipelineName, options);
            }
            return Instance;
        }

        public TSendTransportBuilder ConfigurePipeline(Action<PipelineBuilder> pipeline)
        {
            pipeline?.Invoke(Pipeline);
            return Instance;
        }
    }
}