using System;
using Meceqs.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public abstract class TransportSenderBuilder<TTransportSenderBuilder, TTransportSenderOptions>
        : ITransportSenderBuilder<TTransportSenderBuilder, TTransportSenderOptions>
        where TTransportSenderBuilder : ITransportSenderBuilder<TTransportSenderBuilder, TTransportSenderOptions>
        where TTransportSenderOptions : TransportSenderOptions
    {
        public string PipelineName { get; }

        public IServiceCollection Services { get; }

        /// <inheritdoc/>
        public abstract TTransportSenderBuilder Instance { get; }

        protected TransportSenderBuilder(IServiceCollection services, string pipelineName)
        {
            Guard.NotNull(services, nameof(services));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            Services = services;
            PipelineName = pipelineName;
        }

        public TTransportSenderBuilder Configure(Action<TTransportSenderOptions> options)
        {
            if (options != null)
            {
                Services.Configure(PipelineName, options);
            }

            return Instance;
        }

        public TTransportSenderBuilder ConfigurePipeline(Action<PipelineOptions> pipeline)
        {
            if (pipeline != null)
            {
                Services.Configure(PipelineName, pipeline);
            }
            return Instance;
        }
    }
}