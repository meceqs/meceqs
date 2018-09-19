using System;
using System.Collections.Generic;
using Meceqs.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meceqs.Transport
{
    public abstract class TransportPipelineBuilder
    {
        private readonly List<Action<IPipelineBuilder>> _configurePipelineBuilderDelegates = new List<Action<IPipelineBuilder>>();

        /// <summary>
        /// Gets the Meceqs builder.
        /// </summary>
        public IMeceqsBuilder MeceqsBuilder { get; }

        /// <summary>
        /// Gets the application service collection.
        /// </summary>
        public IServiceCollection Services => MeceqsBuilder.Services;

        /// <summary>
        /// Gets the configuration section for the pipeline being built.
        /// </summary>
        public IConfigurationSection PipelineConfiguration { get; }

        /// <summary>
        /// Gets the name of the pipeline configured by this builder.
        /// </summary>
        public string PipelineName { get; }

        protected TransportPipelineBuilder(IMeceqsBuilder meceqsBuilder, string pipelineName)
        {
            Guard.NotNull(meceqsBuilder, nameof(meceqsBuilder));
            Guard.NotNullOrWhiteSpace(pipelineName, nameof(pipelineName));

            MeceqsBuilder = meceqsBuilder;
            PipelineName = pipelineName;
            PipelineConfiguration = meceqsBuilder.Configuration.GetSection(PipelineName);
        }

        protected void ConfigurePipelineInternal(Action<IPipelineBuilder> pipeline)
        {
            if (pipeline != null)
            {
                _configurePipelineBuilderDelegates.Add(pipeline);
            }
        }

        public virtual void Build()
        {
            Action<IPipelineBuilder> pipelineBuilder = null;
            foreach (var configure in _configurePipelineBuilderDelegates)
            {
                pipelineBuilder += configure;
            }

            MeceqsBuilder.AddPipeline(PipelineName, pipelineBuilder);
        }
    }
}
