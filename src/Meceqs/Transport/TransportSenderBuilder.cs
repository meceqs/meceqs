using System;
using Meceqs.Configuration;
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

        public IMeceqsBuilder MeceqsBuilder { get; }

        public IServiceCollection Services => MeceqsBuilder.Services;

        /// <inheritdoc/>
        public abstract TTransportSenderBuilder Instance { get; }

        protected TransportSenderBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
        {
            Guard.NotNull(meceqsBuilder, nameof(meceqsBuilder));

            MeceqsBuilder = meceqsBuilder;
            PipelineName = pipelineName ?? MeceqsDefaults.SendPipelineName;
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